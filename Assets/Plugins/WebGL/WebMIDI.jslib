mergeInto(LibraryManager.library, {
  WebMIDI_Init: function (gameObjectNamePtr, onMsgMethodPtr, onReadyMethodPtr, onErrorMethodPtr) {
    const goName = UTF8ToString(gameObjectNamePtr);
    const onMsg = UTF8ToString(onMsgMethodPtr);
    const onReady = UTF8ToString(onReadyMethodPtr);
    const onErr = UTF8ToString(onErrorMethodPtr);

    if (!navigator.requestMIDIAccess) {
      SendMessage(goName, onErr, "WebMIDI not supported in this browser.");
      return;
    }

    // Save callback targets globally
    window.__unityWebMIDI = window.__unityWebMIDI || {};
    window.__unityWebMIDI.goName = goName;
    window.__unityWebMIDI.onMsg = onMsg;
    window.__unityWebMIDI.onReady = onReady;
    window.__unityWebMIDI.onErr = onErr;

    // IMPORTANT: call this from a user gesture in practice (button click).
    navigator.requestMIDIAccess({ sysex: false }).then(
      (access) => {
        window.__unityWebMIDI.access = access;

        // Wire inputs
        const hookInput = (input) => {
          input.onmidimessage = (e) => {
            // Pack as JSON string to Unity
            const payload = JSON.stringify({
              deviceId: input.id,
              name: input.name,
              ts: e.timeStamp,
              data: Array.from(e.data) // [status, data1, data2...]
            });
            SendMessage(goName, onMsg, payload);
          };
        };

        access.inputs.forEach(hookInput);

        // Handle hot-plug
        access.onstatechange = (ev) => {
          const port = ev.port;
          if (port.type === "input" && port.state === "connected") hookInput(port);
        };

        SendMessage(goName, onReady, "ok");
      },
      (err) => {
        SendMessage(goName, onErr, (err && err.message) ? err.message : String(err));
      }
    );
  },

  WebMIDI_Send: function (outputIdPtr, b0, b1, b2) {
    const outputId = UTF8ToString(outputIdPtr);
    const m = window.__unityWebMIDI;
    if (!m || !m.access) return;

    const out = m.access.outputs.get(outputId);
    if (!out) return;

    out.send([b0 & 255, b1 & 255, b2 & 255]);
  }
});
