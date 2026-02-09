mergeInto(LibraryManager.library, {
  WebAudio_Resume: function () {
    if (!Module._webaudio_ctx) {
      var AudioContext = window.AudioContext || window.webkitAudioContext;
      Module._webaudio_ctx = new AudioContext();
      Module._webaudio_osc = null;
      Module._webaudio_gain = null;
    }
    var c = Module._webaudio_ctx;
    if (c && c.state === "suspended") c.resume();
  },

  WebAudio_StartSine: function (frequency, amplitude, rampMs) {
    var AudioContext = window.AudioContext || window.webkitAudioContext;

    if (!Module._webaudio_ctx) {
      Module._webaudio_ctx = new AudioContext();
      Module._webaudio_osc = null;
      Module._webaudio_gain = null;
    }

    var c = Module._webaudio_ctx;

    // Stop existing
    if (Module._webaudio_osc) {
      try { Module._webaudio_osc.stop(); } catch (e) {}
      try { Module._webaudio_osc.disconnect(); } catch (e) {}
      Module._webaudio_osc = null;
    }
    if (Module._webaudio_gain) {
      try { Module._webaudio_gain.disconnect(); } catch (e) {}
      Module._webaudio_gain = null;
    }

    var osc = c.createOscillator();
    var gain = c.createGain();

    osc.type = "sine";

    var f = Math.max(20.0, Math.min(20000.0, frequency));
    var a = Math.max(0.0, Math.min(1.0, amplitude));
    var ramp = Math.max(0.0, rampMs) / 1000.0;

    var now = c.currentTime;

    osc.frequency.setValueAtTime(f, now);

    gain.gain.setValueAtTime(0.0, now);
    gain.gain.linearRampToValueAtTime(a, now + ramp);

    osc.connect(gain);
    gain.connect(c.destination);

    osc.start(now);

    Module._webaudio_osc = osc;
    Module._webaudio_gain = gain;
  },

  WebAudio_SetFrequency: function (frequency, rampMs) {
    var c = Module._webaudio_ctx;
    var osc = Module._webaudio_osc;
    if (!c || !osc) return;

    var f = Math.max(20.0, Math.min(20000.0, frequency));
    var ramp = Math.max(0.0, rampMs) / 1000.0;

    var now = c.currentTime;
    osc.frequency.cancelScheduledValues(now);
    osc.frequency.setValueAtTime(osc.frequency.value, now);
    osc.frequency.linearRampToValueAtTime(f, now + ramp);
  },

  WebAudio_SetAmplitude: function (amplitude, rampMs) {
    var c = Module._webaudio_ctx;
    var gain = Module._webaudio_gain;
    if (!c || !gain) return;

    var a = Math.max(0.0, Math.min(1.0, amplitude));
    var ramp = Math.max(0.0, rampMs) / 1000.0;

    var now = c.currentTime;
    gain.gain.cancelScheduledValues(now);
    gain.gain.setValueAtTime(gain.gain.value, now);
    gain.gain.linearRampToValueAtTime(a, now + ramp);
  },

  WebAudio_StopSine: function (rampMs) {
    var c = Module._webaudio_ctx;
    var osc = Module._webaudio_osc;
    var gain = Module._webaudio_gain;
    if (!c || !osc || !gain) return;

    var ramp = Math.max(0.0, rampMs) / 1000.0;
    var now = c.currentTime;

    gain.gain.cancelScheduledValues(now);
    gain.gain.setValueAtTime(gain.gain.value, now);
    gain.gain.linearRampToValueAtTime(0.0, now + ramp);

    var stopTime = now + ramp + 0.01;
    try { osc.stop(stopTime); } catch (e) {}

    // Clean up after stop
    setTimeout(function () {
      try { osc.disconnect(); } catch (e) {}
      try { gain.disconnect(); } catch (e) {}
      if (Module._webaudio_osc === osc) Module._webaudio_osc = null;
      if (Module._webaudio_gain === gain) Module._webaudio_gain = null;
    }, Math.ceil((ramp + 0.05) * 1000));
  }
});
