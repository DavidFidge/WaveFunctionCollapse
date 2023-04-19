namespace FrigidRogue.WaveFunctionCollapse;

public class NextStepResult
{
    public bool IsComplete { get; set; }
    public bool IsFailed { get; set; }

    public bool IsContinue => !IsComplete && !IsFailed;
    
    public static NextStepResult Complete()
    {
        return new NextStepResult { IsComplete = true };
    }
    
    public static NextStepResult Failed()
    {
        return new NextStepResult { IsFailed = true };
    }
    
    public static NextStepResult Continue()
    {
        return new NextStepResult();
    }
}