
[System.Serializable]
public class S_QuestStepState
{
    public string state;
    public string status;

    // Constructor
    public S_QuestStepState(string state, string status)
    {
        this.state = state;
        this.status = status;
    }

    // Default constructor
    public S_QuestStepState()
    {
        this.state = "";
        this.status = "";
    }
}
