%MLReadWavFile.m
%
% MLReadWavFile('wavfile');
%
function [xData, fs] = MLReadWavFile(WavName)
[x fs] = audioread(WavName);
xData = x(:, 1);
