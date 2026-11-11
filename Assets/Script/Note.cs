#region Modifier
// AIM          (minRange   |   maxRange)

// Combo    
// HoldCombo    (AutoCombo = true   |   LoopCombo = true)
// (MaxCombo   | ComboWindow) 

// Charge       (ChargeLimit = 3 |   ChargeMax = 3)
#endregion

#region LifeCycle
// Toggle           |   Aura            |   Stance          |   Mode
// (CastTime = 0f   |   Duration = -1f  |   Cooldown = x)

// Cast             |   Burst           |   Invoke          |   Trigger
// (CastTime = 0.5f |   Duration = 0f   |   Cooldown = X)

// Active           |   Enhance         |   Channel         |   
// (CastTime = 0f   |   Duration = 3f   |   Cooldown = X)

// CastActive       |   Ritual
// (CastTime = 1f   |   Duration = 5f   |   Cooldown = X)
#endregion

#region
// OnBeginCast 
    // Play Anim 
    // Face Target
    // Create Indicator

// OnCharge
    // Update Charge FX 
    // Update UI 
    // Lock Movement 

// OnRelease 
    // Play Attack Anim
    // Spawn Proj
    // Enable Hitbox 
    // Set Forced Combo 

// OnCasting
    // Update Casting FX 
    // Update Beam Dir
    // Update Charge Visual 

// OnActive 
    // DOT
    // Aura 
    // Channel Beam 
    // Orbit Projectile 
    // Summon 

// OnEnd
    // Cleanup FX 
    // Reset State 
    // Start Cooldown 
    // Return To Idle

// OnCancel
    // Cleanup FX 
    // Disable Hitbox 
    // Stop Animation
    // Reset AIM
#endregion
