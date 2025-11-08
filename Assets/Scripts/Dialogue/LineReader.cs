using UnityEngine;
using Yarn.Unity;

public class LineReader : DialoguePresenterBase
{
    [SerializeField] private SpriteBehavior spriteBehavior;

    private bool hasShownPortraitsAfterChoice = false;

    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken cancellationToken)
    {
        
        string text = line.TextWithoutCharacterName.Text;
        string speakerName = line.CharacterName;

        if (speakerName == "Null")
            speakerName = "";

        if (!string.IsNullOrEmpty(speakerName))
            spriteBehavior.SetActiveSpeaker(speakerName);
        
        
        // After a choice, restore portraits once
        if (!hasShownPortraitsAfterChoice && spriteBehavior != null)
        {
            spriteBehavior.ShowPortraits();
            hasShownPortraitsAfterChoice = true;
        }

        if (Debug.isDebugBuild) Debug.Log($"LineReader: {speakerName} says \"{text}\"");

        AudioManager.instance.PlayUI(UI.CLICK);
        return YarnTask.CompletedTask;
    }

#pragma warning disable CS8632
    public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] options, System.Threading.CancellationToken cancellationToken)
#pragma warning restore CS8632
    {
        //Hide portraits when options are shown
        if (spriteBehavior != null)
            spriteBehavior.HidePortraits();

        hasShownPortraitsAfterChoice = false;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        return YarnTask.FromResult<DialogueOption?>((DialogueOption?)null);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}