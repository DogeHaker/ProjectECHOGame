# VHS Post Process Effect (Unity)

A lightweight VHS-style post-processing effect for Unity that simulates analog video artifacts using a video overlay and shader-based distortion.

Designed to be simple, customizable, and usable directly in the editor (no Play mode required).
![preview](demos/preview.gif)
![preview](demos/preview1.gif)

---

## ✨ Features

- 📺 VHS-style glitch overlay using a VideoClip
- 🎛 Adjustable **intensity** of the effect
- 🔍 **Scale control** for zooming the VHS texture
- ↔️ **Offset control (X/Y)** for positioning the overlay
- ⚡ Runs in **Edit Mode** (no need to press Play)
- 🧼 Automatically hides the VideoPlayer component from Inspector
- 🎞 Looping video playback with no audio overhead
- 🧪 Lightweight and easy to tweak

---

## 📦 Requirements

- Unity 2022 (Built-in Render Pipeline)
- A Camera with this script attached
- A VHS-style VideoClip included 4 (noise/glitch texture, mp4 format)

---

## 🚀 Setup

1. Add the script to your **Main Camera**
2. Assign: The Script to your scene camera and give it the shader input then assign a glitch clip for overlay and you're done!
   - `Shader` → your VHS shader
   - `VHS Clip` → your glitch/noise video
   - Scale, Offset , Intensity and Adjust!
3. Done ✅

The effect should appear immediately (even in Edit Mode).

---

## 🎚 Parameters

### Intensity

Controls how strong the VHS overlay is.

- `0` → no effect
- `1` → default
- `>1` → stronger / more aggressive

---

### Scale

Controls the size of the VHS texture.

- `<1` → zooms **in**
- `1` → normal
- `>1` → zooms **out**

---

### Offset (X, Y)

Moves the VHS overlay across the screen.

- X → horizontal shift
- Y → vertical shift

Useful for aligning or stylizing the effect.

---

## ⚙️ How It Works

- A hidden **VideoPlayer** component plays a looping VHS-style clip
- The shader samples this texture (`_VHSTex`)
- Scanline distortion + noise is applied in screen space
- Final image = Camera render + VHS overlay

---

## 🧼 Inspector Behavior

The `VideoPlayer` is:

- Automatically added at runtime
- Hidden from the Inspector (`HideFlags`)

This keeps your camera clean while still using video internally.

---

## ⚠️ Notes (Bugs)

- The effect depends on the assigned VideoClip — results vary based on source
- If the effect doesn't update in Edit Mode, try:
  - Moving the camera slightly
  - Re-enabling the component
  - Saving causes camera to go dark just hit play or re-enable the camera thats all.

---

## 🛠 Customization Ideas

- RGB channel offset (chromatic aberration)
- Scanline thickness control
- Noise intensity slider
- Horizontal tearing strength
- Color grading / tint

---

## 📄 License

Free to use / modify. No warranty.

---

## 🤝 Credits and Inspiration

Inspired by old VHS tape effects, PuppetCombo

- [![@GlaireDaggers|64](https://avatars.githubusercontent.com/u/8466762?s=64&v=4)](https://github.com/GlaireDaggers)[**GlaireDaggers** Hazel Stagner](https://github.com/GlaireDaggers)
- [![@radiatoryang](https://avatars.githubusercontent.com/u/2285943?s=64&v=4)](https://github.com/radiatoryang)[**radiatoryang** Robert Yang](https://github.com/radiatoryang)
