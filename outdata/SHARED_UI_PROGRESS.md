# Shared UI — first implementation

Implemented runtime Settings overlay in Home, Upgrade, Relics and CharacterLab without rebuilding scenes. Battle overlay pauses scaled gameplay and restores the previous speed. Resume, Restart/Home confirmations and Escape handling are present. Sound Effects preference drives a synthesized click for the new overlay buttons. Vibration is available on mobile only and disabled on desktop. Preferences use separate PlayerPrefs keys; progress JSON is untouched. Music explicitly reports that no tracks are installed.

RollPanel presentation now waits while this pause overlay is open, including hold and fade; reward RNG and payout logic are unchanged.

Validation: 68 C# source files compile with tools/csharp_check.sh. Actual Unity interaction, screen placement, mobile vibration and x1/x2/x3 resume still require Play-mode testing. New artwork and styling approval, feedback on all existing buttons, and shared insufficient-currency popups remain pending. No scene or background was replaced in this pass.
