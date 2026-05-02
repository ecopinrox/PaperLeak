using System.Collections.Generic;

/*
 * DTO created to prevent older saves from breaking in newer updates.
 *
 * Updates to the game are assumed to only add fields to the save objects, not remove or change any existing fields, meaning that save files from previous versions will not contain data for newer fields. Thus, the JSON is read into this DTO and the program can skip overwriting fields in the MasterSave that correspond to null entries.
 */

public class MasterSaveDto
{
    public int? currentLevelIndex;
    public int? difficulty;
    public HashSet<int> visited;
    public SaveStateDto[] levelStates;
}

