using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    ServerScoreReplicator m_ScoreTracker1;
    [SerializeField]
    ServerScoreReplicator m_ScoreTracker2;
    //[SerializeField] System.Collections.Generic.List<ServerScoreReplicator> m_ScoreTrackerList; //WIP: more generalised score tracker system

    [SerializeField]
    UIDocument m_InGameUIDocument;

    VisualElement m_InGameRootVisualElement;

    TextElement m_ScoreText1;
    TextElement m_ScoreText2;

    TextElement m_JoinCodeText;

    void Awake()
    {
        m_InGameRootVisualElement = m_InGameUIDocument.rootVisualElement;

        m_ScoreText1 = m_InGameRootVisualElement.Query<TextElement>("Score1Text");
        m_ScoreText2 = m_InGameRootVisualElement.Query<TextElement>("Score2Text");
        m_JoinCodeText = m_InGameRootVisualElement.Query<TextElement>("JoinCodeText");
    }

    void Start()
    {
        OnScoreChanged(0, m_ScoreTracker1.ReplicatedScore.Value);
        m_ScoreTracker1.ReplicatedScore.OnValueChanged += OnScoreChanged;

        OnScoreChanged(0, m_ScoreTracker2.ReplicatedScore.Value);
        m_ScoreTracker2.ReplicatedScore.OnValueChanged += OnScoreChanged;
    }

    void OnDestroy()
    {
        m_ScoreTracker1.ReplicatedScore.OnValueChanged -= OnScoreChanged;

        m_ScoreTracker2.ReplicatedScore.OnValueChanged -= OnScoreChanged;
    }

    void OnScoreChanged(int previousValue, int newValue)
    {
        m_ScoreText1.text = $"{m_ScoreTracker1.Score}";
        m_ScoreText2.text = $"{m_ScoreTracker2.Score}";
    }

    public void OnJoinCodeReceived(string newValue)
    {
        m_JoinCodeText.text = newValue;
    }
}
