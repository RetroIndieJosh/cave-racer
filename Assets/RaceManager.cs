using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance = null;

    [SerializeField]
    private float m_timeScale = 1.0f;

    [SerializeField]
    private TextMeshProUGUI m_waitingTextMesh = null;

    [SerializeField]
    private TextMeshProUGUI m_resultsTextMesh = null;

    [SerializeField]
    private int m_lapCount = 3;

    [SerializeField]
    private float m_waitForFinishSec = 30.0f;

    public bool HasStarted = false;
    public int LapCount {  get { return m_lapCount; } }

    private List<Racer> m_finisherList = new List<Racer>();

    private float m_timeSinceFirstFinish = 0.0f;

    public void Finish( Racer a_racer ) {
        m_finisherList.Add( a_racer );
        a_racer.IsFinished = true;
        a_racer.Stop();

        if ( m_finisherList.Count == 1 )
            m_waitingTextMesh.gameObject.SetActive( true );

        if ( a_racer == RaceController.instance.CurrentRacer ) {
            if ( RaceController.instance.IsPlayer ) ShowResults();
            RaceController.instance.NextRacer();
        }
    }

    private void Awake() {
        Time.timeScale = m_timeScale;

        instance = this;
        m_waitingTextMesh.gameObject.SetActive( false );
        m_resultsTextMesh.gameObject.SetActive( false );
    }

    private void Update() {
        if ( m_finisherList.Count == 0 ) return;
        m_timeSinceFirstFinish += Time.deltaTime;
        if ( m_timeSinceFirstFinish > m_waitForFinishSec ) EndRace();

        var remainingCount = FindObjectsOfType<Racer>().Length - m_finisherList.Count;
        m_waitingTextMesh.text = "";
        if ( RaceController.instance.IsPlayer == false )
            m_waitingTextMesh.text += string.Format( "{0} CAM\n\n", RaceController.instance.CurrentRacer.name );
        m_waitingTextMesh.text += string.Format( "{0} wins! Waiting for {1} players to finish. Disqualify in {2:0.0}s",
            m_finisherList[0].name, remainingCount, m_waitForFinishSec - m_timeSinceFirstFinish );

        if ( m_resultsShowing ) UpdateResults();

        if( m_resultsShowing && Input.GetButtonDown("Dig") ) {
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }

    private bool m_resultsShowing = false;
    private bool m_isFinished = false;

    private void EndRace() {
        if ( RaceController.instance.IsPlayer ) ShowResults();

        foreach ( var racer in FindObjectsOfType<Racer>() ) {
            if ( m_finisherList.Contains( racer ) == false ) {
                racer.IsFinished = true;
                racer.Stop();
            }
        }

        m_isFinished = true;
    }

    private void ShowResults() {
        m_waitingTextMesh.gameObject.SetActive( false );
        m_resultsShowing = true;
    }

    private void UpdateResults() {
        if( Input.GetAxis("Accelerate") > Mathf.Epsilon) {
            m_resultsTextMesh.text = "";
            return;
        }

        m_resultsTextMesh.text = "<u>Results</u>\n\n";
        var rank = 0;
        var resultsList = m_finisherList.OrderBy( o => o.TotalTime );
        foreach( var racer in resultsList) {
            ++rank;
            m_resultsTextMesh.text += string.Format( "{0}. <b>{1}</b> : {2}\n", rank, racer.name, racer.TotalTimeStr );
        }
        m_resultsTextMesh.text += "\n";
        foreach ( var racer in FindObjectsOfType<Racer>() ) {
            if ( m_finisherList.Contains( racer ) == false ) {
                if( racer.IsFinished) 
                    m_resultsTextMesh.text += string.Format( "X. <b>{0}</b> : DISQUALIFIED\n", racer.name );
                else m_resultsTextMesh.text += string.Format( "X. <b>{0}</b> : STILL RACING\n", racer.name );
            }
        }
        m_resultsTextMesh.text += "\nPress START or R to play again";
        if( m_isFinished == false )
            m_resultsTextMesh.text += "\nPress JUMP to change camera\nHold ACCELERATE to hide text";
        m_resultsTextMesh.gameObject.SetActive( true );
    }
}
