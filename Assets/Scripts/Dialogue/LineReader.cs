using UnityEngine;
using Yarn.Unity;
using System.Threading;

public class LineReader : DialoguePresenterBase
{
    [SerializeField] private SpriteBehavior spriteBehavior;

    public override YarnTask OnDialogueStartedAsync()
    {
        // Nothing special when dialogue starts
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        // Nothing special when dialogue ends
        return YarnTask.CompletedTask;
    }

    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken cancellationToken)
    {
        string text = line.TextWithoutCharacterName.Text;
        string speakerName = line.CharacterName;

        if (!string.IsNullOrEmpty(speakerName))
        {
            spriteBehavior.SetActiveSpeaker(speakerName);
        }

        Debug.Log($"LineReader: {speakerName} says \"{text}\"");

        // // Instantly return so dialogue continues
        return YarnTask.CompletedTask;
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] options, CancellationToken cancellationToken)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        // 👇 Hide portraits when the player has to make a choice
        if (spriteBehavior != null)
        {
            spriteBehavior.HidePortraits();
        }

        // This presenter doesn't handle options — 
        // the actual OptionsPresenter will show them.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        return YarnTask.FromResult<DialogueOption?>(null);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
