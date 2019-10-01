function [fs, ch, totSamp, dur, bits] = MLWavInfo(WavName)
p = audioinfo(WavName);
fs = p.SampleRate;
ch = p.NumChannels;
totSamp = p.TotalSamples;
dur = p.Duration;
bits = p.BitsPerSample;