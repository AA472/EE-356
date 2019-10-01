function MLPlayWavFile(WavName)
[x fs] = audioread(WavName);
p = audioplayer(x, fs);
playblocking(p);
return;