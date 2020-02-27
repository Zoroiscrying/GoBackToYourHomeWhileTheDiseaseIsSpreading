using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsController : Singleton<WindowsController>
{
    public int ActivePeopleMax = 6;
    public List<WindowPeople> WindowPeoples = new List<WindowPeople>();
    private List<WindowPeople> m_activePeople = new List<WindowPeople>();
    private List<WindowPeople> m_inactivePeople = new List<WindowPeople>();

    public int ActivePeopleMaxWithLevel
    {
        get { return ActivePeopleMax + Mathf.RoundToInt(Gamemanager.Instance.DifficultyLevel * 4); }
    }
    
    public void RemoveActive(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var randomNum = Random.Range(0, 100);
            
            if (m_activePeople.Count > 0)
            {
                //active to inactive
                var tempPeople1 = m_activePeople[randomNum % m_activePeople.Count];
                m_activePeople.RemoveAt(randomNum % m_activePeople.Count);
                tempPeople1.DeActivate();
                
                m_inactivePeople.Add(tempPeople1);
            }
        }
    }

    public void AllActive()
    {
        foreach (var people in m_inactivePeople)
        {
            people.Activate();
            m_activePeople.Add(people);
        }
        
        m_inactivePeople.Clear();
    }

    public void AllDeActive()
    {
        foreach (var people in m_activePeople)
        {
            people.DeActivate();
            m_inactivePeople.Add(people);
        }
        
        m_activePeople.Clear();
    }

    public void RegulateActivatedPeople()
    {
        if (m_activePeople.Count < ActivePeopleMaxWithLevel && m_inactivePeople.Count > 0)
        {
            AddActive(ActivePeopleMaxWithLevel - m_activePeople.Count);
        }
        else if (m_activePeople.Count > ActivePeopleMaxWithLevel)
        {
            RemoveActive(m_activePeople.Count - ActivePeopleMaxWithLevel);
        }
    }
    
    public void AddActive(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var randomNum = Random.Range(0, 100);
            
            if (m_activePeople.Count >= ActivePeopleMaxWithLevel)
            {
                //active to inactive
                var tempPeople1 = m_activePeople[randomNum % m_activePeople.Count];
                m_activePeople.RemoveAt(randomNum % m_activePeople.Count);
                tempPeople1.DeActivate();
                //inactive to active
                var tempPeople2 = m_inactivePeople[randomNum % m_inactivePeople.Count];
                m_inactivePeople.RemoveAt(randomNum % m_inactivePeople.Count);
                tempPeople2.Activate();
                
                m_activePeople.Add(tempPeople2);
                m_inactivePeople.Add(tempPeople1);
            }
            else if (m_inactivePeople.Count > 0)
            {
                var tempPeople = m_inactivePeople[randomNum % m_inactivePeople.Count];
                m_inactivePeople.RemoveAt(randomNum % m_inactivePeople.Count);
                tempPeople.Activate();
                m_activePeople.Add(tempPeople);
                
            }
        }
    }

    public void GameStart()
    {
        AllDeActive();
    }
    
    
    // Start is called before the first frame update
    public void Start()
    {
        this.m_inactivePeople = WindowPeoples;
        GameStart();
    }

    private int _intTimer = 0;
    private float _floatTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        _floatTimer += Time.deltaTime;
        _intTimer = Mathf.RoundToInt(_floatTimer);

        if (_intTimer%1 == 0)
        {
            RegulateActivatedPeople();
        }
    }
}
