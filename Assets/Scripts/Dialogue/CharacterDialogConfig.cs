using System.Collections.Generic;

public static class CharacterDialogConfig
{
    private static Dictionary<DialogActor, CommonSfx> _specialVoiceClips = new()
    {
        // High talking
        { DialogActor.missMiau, CommonSfx.TextH },
        { DialogActor.pfefferPig, CommonSfx.TextH },
        { DialogActor.zoeZiege, CommonSfx.TextH },
        { DialogActor.peterPiep, CommonSfx.TextH }, // maybe custom bird tweet sounds?
        
        // Special SFX
        { DialogActor.telephone, CommonSfx.Call },
    };

    public static CommonSfx GetCharacterVoice(DialogActor actor)
    {
        return _specialVoiceClips.GetValueOrDefault(actor, CommonSfx.TextM);
    }
}

public enum DialogActor
{
    DetectiveMiez,
    henryHabicht,
    missMiau,
    brunoBör,
    karlNikel,
    kurtKroko,
    pfefferPig,
    pherdiPhuchs,
    professorBello,
    zoeZiege,
    telephone,
    peterPiep
}