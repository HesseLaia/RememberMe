import math
import random
import subprocess
import sys
from array import array
from pathlib import Path

RATE = 44100
TAU = math.tau
ROOT = Path(__file__).resolve().parents[1] / "game" / "audio"
RANDOM = random.Random(5190)


def stereo(seconds):
    return [[0.0] * round(seconds * RATE) for _ in range(2)]


def voice(track, start, midi, level, pan=0.0, bell=False):
    frequency = 440 * 2 ** ((midi - 69) / 12)
    length = 2.8 if bell else 4.2
    begin = round(start * RATE)
    space = len(track[0])
    partials = (1.0, 2.76, 5.39) if bell else (1.0, 2.002, 3.008)
    decay = 2.2 if bell else 1.4
    spread = (math.sqrt((1-pan)/2), math.sqrt((1+pan)/2))
    end = round(length * RATE)
    for sample in range(end):
        time = sample / RATE
        angle = TAU * frequency * time
        envelope = min(1, time * (250 if bell else 95)) * math.exp(-time * decay)
        envelope *= min(1, (end - sample) / (0.55 * RATE))
        resonance = math.sin(angle) + 0.24 * math.sin(angle * partials[1]) * math.exp(-time * 5)
        resonance += 0.1 * math.sin(angle * partials[2]) * math.exp(-time * 10)
        value = level * envelope * resonance
        current = (begin + sample) % space
        echo = (current + 5150) % space
        track[0][current] += value * spread[0]
        track[1][current] += value * spread[1]
        track[0][echo] += value * spread[1] * 0.14
        track[1][echo] += value * spread[0] * 0.14


def score(kind):
    beat = {"Title": 0.68, "Farewell": 0.82, "Flashback": 0.77, "Keepsake": 0.57}[kind]
    bars = 16 if kind == "Farewell" else 12 if kind == "Title" else 8
    track = stereo(bars * 3 * beat)
    roots = (50, 46, 53, 48, 50, 46, 55, 45) if kind != "Title" else (53, 48, 50, 46, 53, 45, 46, 48)
    motif = ((0, 69), (1.2, 67), (2.1, 65), (4.0, 62), (6, 65), (7.1, 67), (8.1, 69), (10.5, 65))
    for bar in range(bars):
        root = roots[bar % len(roots)]
        if kind == "Keepsake":
            voice(track, bar * 3 * beat, root + 12, 0.16, -0.3, True)
        else:
            voice(track, bar * 3 * beat, root, 0.18 if kind != "Flashback" else 0.12, -0.27)
            if kind != "Flashback" or bar % 2 == 0:
                voice(track, (bar * 3 + 1.5) * beat, root + 7, 0.10, 0.24)
    for block in range(bars // 4):
        for offset, note in motif:
            if kind == "Flashback" and offset not in (0, 4.0, 8.1):
                continue
            if kind == "Farewell" and block % 2 == 1 and offset in (1.2, 7.1):
                continue
            pitch = note + (12 if kind == "Keepsake" else -12 if kind == "Flashback" else 0)
            voice(track, (block * 12 + offset) * beat, pitch, 0.32 if kind == "Keepsake" else 0.21,
                  0.3 if offset % 2 else -0.28, kind == "Keepsake")
    return track


def atmosphere(rain):
    track = stereo(24 if rain else 18)
    for channel, plane in enumerate(track):
        slow = fast = 0
        for index in range(len(plane)):
            noise = RANDOM.uniform(-1, 1)
            fast += (noise - fast) * (0.32 if rain else 0.05)
            slow += (noise - slow) * 0.005
            time = index / RATE
            swell = 0.83 + 0.14 * math.sin(TAU * time / 8 + channel * 0.3)
            edge = min(1, index / 1400, (len(plane) - index) / 1400)
            plane[index] = edge * swell * (fast * (0.21 if rain else 0.10) + slow * 0.36)
    if rain:
        for _ in range(28):
            voice(track, RANDOM.uniform(0, 24), RANDOM.randrange(76, 95), 0.012, RANDOM.uniform(-0.8, 0.8), True)
    return track


def effect(kind):
    duration = {"Tap": 0.18, "Grass": 0.24, "Stone": 0.25, "Wood": 0.24, "Lid": 0.66, "Drip": 0.62}[kind]
    track = stereo(duration)
    if kind == "Drip":
        voice_track = stereo(2.8)
        voice(voice_track, 0, 84, 0.25, 0.25, True)
        voice(voice_track, 0.12, 91, 0.09, -0.17, True)
        track = [plane[:len(track[0])] for plane in voice_track]
    else:
        layers = {
            "Tap": [(0.0, 640, 44, 0.22, 0.10)],
            "Grass": [(0.0, 84, 27, 0.12, 0.26), (0.046, 118, 44, 0.05, 0.20)],
            "Stone": [(0.0, 132, 40, 0.24, 0.22), (0.045, 216, 67, 0.10, 0.17)],
            "Wood": [(0.0, 112, 36, 0.28, 0.11), (0.027, 185, 52, 0.10, 0.07)],
            "Lid": [(0.02, 80, 10, 0.10, 0.18), (0.14, 144, 18, 0.13, 0.17),
                    (0.28, 509, 36, 0.18, 0.08)],
        }[kind]
        for onset, frequency, decay, body, grit in layers:
            filtered = 0
            for index in range(round(onset * RATE), len(track[0])):
                time = index / RATE - onset
                filtered += (RANDOM.uniform(-1, 1) - filtered) * (0.72 if kind == "Grass" else 0.3)
                value = (body * math.sin(TAU * frequency * time * (1 - time * 0.17)) +
                         grit * filtered) * math.exp(-time * decay) * max(0, min(1, time * 950))
                track[0][index] += value
                track[1][index] += value * (0.87 if kind == "Lid" else 0.96)
    for plane in track:
        for end in range(min(len(plane), 1200)):
            plane[-1-end] *= end / 1200
    return track


def export(ffmpeg, folder, name, track):
    pcm = array("h", (round(max(-0.98, min(0.98, value)) * 32767) for frame in zip(*track) for value in frame))
    samples = pcm.tobytes()
    for extension, options in (("ogg", ("-c:a", "libvorbis", "-q:a", "5")),
                               ("m4a", ("-c:a", "aac", "-b:a", "144k", "-movflags", "+faststart"))):
        destination = ROOT / folder / f"RM_Story{name}.{extension}"
        subprocess.run([ffmpeg, "-hide_banner", "-loglevel", "error", "-y", "-f", "s16le",
                        "-ar", str(RATE), "-ac", "2", "-i", "-", *options, str(destination)],
                       input=samples, check=True)
    print(f"{folder}/RM_Story{name}", flush=True)


if __name__ == "__main__":
    for name in ("Title", "Farewell", "Flashback", "Keepsake"):
        export(sys.argv[1], "bgm", name, score(name))
    for name in ("Rain", "Room"):
        export(sys.argv[1], "bgs", name, atmosphere(name == "Rain"))
    for name in ("Tap", "Grass", "Stone", "Wood", "Lid", "Drip"):
        export(sys.argv[1], "se", name, effect(name))
