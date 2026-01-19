# AudioManager (FMOD Integration)

Lightweight central audio controller for Unity + FMOD. Handles one‑shots, music transitions, and managed looping sounds with safe async fade logic.

## Features
* One‑shot SFX (2D & 3D overloads)
* BGM play / stop with fade, crossfade (delay=0) or sequential (delay >= fadeDuration)
* Cancellation-safe transitions (new call cancels current fade task)
* Loop sounds tracked by ID (optional GameObject attachment for 3D positioning)
* Automatic cleanup on destroy (stop + release)
* Lazy singleton (`AudioManager.Instance`) with `DontDestroyOnLoad`
* Dependency Injection friendly.

## Quick Use
```csharp
// Initialize AudioManager. This happens automatically if you have prefab AudioManager in scene.
// IMPORTANT: Wait for AudioManager to finish initializing before calling any Save/Load/NewGame APIs.
Task WaitForInitialization();

void PlayOneShot(EventReference reference);
void PlayOneShot(string path);
void PlayOneShot(EventReference reference, Vector3 pos);
void PlayOneShot(string path, Vector3 pos);

void PlayBGM(EventReference reference, float fadeDuration = 1f, float delay = 0f);
void PlayBGM(string path, float fadeDuration = 1f, float delay = 0f);
void StopBGM(float fadeDuration = 1f, float delay = 0f);

EventInstance PlayLoop(string id, string path, GameObject attachedObject = null);
EventInstance PlayLoop(string id, EventReference reference, GameObject attachedObject = null);
void PauseLoop(string id);
void ResumeLoop(string id);
void StopLoop(string id, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT);
```

## Editor Tools: Bus/EventReference Codegen
This package provides 2 Unity Editor windows to map FMOD Studio Buses / Events into strongly-typed C# accessors (easy to call and less error-prone than raw paths).

### Bus Manager
- Open: `Tools/ThanhDV/Audio Manager - FMOD/Bus Manager`
- Fill the bus list (created in FMOD Studio):
    - **Key**: the friendly name you will use in code.
    - **Bus Path**: the FMOD bus path (e.g. `bus:/`, `bus:/SFX`).
- Click **Clean & Save** to:
    - Remove empty entries / duplicate keys
    - Save into the `FMODReferences` asset
    - Auto-generate `FMODBus` at `Assets/Plugins/AudioManager/FMOD/Scripts/FMODBus.cs`

Usage example:
```csharp
var sfxBus = FMODBus.SFX;
```

### EventReference Manager
- Open: `Tools/ThanhDV/Audio Manager - FMOD/EventReference Manager`
- Fill the event list (created in FMOD Studio):
    - **Key**: the friendly name you will use in code.
    - **EventReference**: pick/paste the FMOD event (e.g. path: `event:/UI/Click`).
- Click **Clean & Save** to:
    - Remove empty entries / duplicate keys
    - Save into the `FMODReferences` asset
    - Auto-generate `FMODEventReference` at `Assets/Plugins/AudioManager/FMOD/Scripts/FMODEventReference.cs`

Usage example:
```csharp
AudioManager.Instance.PlayOneShot(FMODEventReference.UI_Click);
```

### Notes
- Property names are generated from **Key**: invalid characters/spaces are converted to `_` (e.g. `UI Click` -> `UI_Click`). Duplicate keys will be suffixed (`_2`, `_3`, ...).
- At runtime, the wrapper resolves values via `AudioManager.Instance.FMODReferences`, so make sure AudioManager is initialized before using `FMODBus.*` / `FMODEventReference.*`.

## Dependency Injection (DI) Integration
For projects using a Dependency Injection framework (like Reflex, VContainer, Zenject, etc.), you can make `AudioManager` DI-friendly by defining the `INJECTION_ENABLED` scripting symbol in your project settings.

**How it works:**
- When `INJECTION_ENABLED` is defined, the standard singleton (`GameSaver.Instance`) is disabled.
- This allows you to register `AudioManager` with your DI container and inject it into other classes instead of accessing it globally.

**Example (using a generic DI container):**
```csharp
// In your Reflex installer or composition root
var audioManagerObject = new GameObject("AudioManager");
var audioManagerInstance = audioManagerObject.AddComponent<AudioManager>();

builder.AddSingleton(audioManagerInstance);

// In your consumer class
public class MyGameManager
{
    [Inject] private readonly AudioManager _audioManager;

    public MyGameManager(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void DoSomething()
    {
        _audioManager.PlayOneShot("event");
    }
}
```