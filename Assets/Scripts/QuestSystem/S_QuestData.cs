using UnityEngine;

[System.Serializable]
public class S_QuestData
{
    public E_QuestState state;
    public int index;
    public S_QuestStepState[] questStepStates;


    /**
     * constructor
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @param	e_queststate	state          	
     * @param	int         	index          	
     * @param	mixed       	S_QuestStepStat	
     * @return	void
     */
    public S_QuestData(E_QuestState state, int index, S_QuestStepState[] questStepStates)
    {
        this.state = state;
        this.index = index;
        this.questStepStates = questStepStates;
    }

    public S_QuestData GetQuestData()
    {
        return new S_QuestData(this.state, this.index, this.questStepStates);
    }

}
