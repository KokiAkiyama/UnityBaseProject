using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] CharacterDatabase database;

    [SerializeField] List<CharacterBrain> characters = new List<CharacterBrain>();
    public List<CharacterBrain> Characters=>characters;
    public CharacterData GetChatacrerData(CharacterIDs id) => database.GetChatacrerData(id);

    public void Copy(ref CharacterData copiedData,CharacterIDs id)=>database.Copy(ref copiedData,id);

    void OderByTurnCharacterList()
    {
        characters=characters.OrderByDescending(character=>character.Param.dexterity).ToList();
    }

    public void AddCharacter(CharacterBrain character)
    {
        if(characters.Contains(character)){return;}
        characters.Add(character);
        OderByTurnCharacterList();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
