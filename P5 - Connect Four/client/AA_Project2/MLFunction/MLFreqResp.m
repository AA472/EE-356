%MLFreqResp.m
%Returns the Fourier transform of a wav file from 
%   0 to fs.
function [xData, f] = MLFreqResp(WavName)
[x fs] = audioread(WavName);
y = x(:, 1);
T = 1/fs;
k = 1:length(y);
xData = fft(y);
xData = abs(xData/max(abs(xData)));
f = k*fs/length(xData);
