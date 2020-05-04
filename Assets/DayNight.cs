using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour {
    [SerializeField]
    private Color m_dayColor = Color.cyan;

    [SerializeField]
    private Color m_nightColor = Color.grey;

    [SerializeField]
    private float m_wholeDayLengthSeconds = 10.0f;

    [SerializeField]
    [Tooltip( "Brightest time in 24 hour format" )]
    [Range(0.0f, 24.0f)]
    private float m_brightestTimeHours = 13;

    [SerializeField]
    [Tooltip( "Darkest time in 24 hour format" )]
    [Range(0.0f, 24.0f)]
    private float m_darkestTimeHours = 2;

    [SerializeField]
    [Tooltip( "Start time in 24 hour format" )]
    private float m_startTime = 12.0f;

    [SerializeField]
    [Tooltip( "Fog at the brightest point of the day" )]
    private float m_fogMin = 0.02f;

    [SerializeField]
    [Tooltip( "Fog at the darkest point of the day" )]
    private float m_fogMax = 0.2f;

    private float m_dayPercent {  get { return m_timeElapsed / m_wholeDayLengthSeconds; } }

    // sunrise and sunset converted to a scale of 0 - m_dayLengthSeconds
    private float m_darkestTime;
    private float m_brightestTime;

    private float m_brightLength;
    private float m_darkLength;

    public bool isBrightening { get { return m_timeElapsed > m_darkestTime && m_timeElapsed <= m_brightestTime; } }

    public string currentTime12 {
        get {
            var timeSpan = System.TimeSpan.FromDays( m_dayPercent );
            System.DateTime time = System.DateTime.Today.Add( timeSpan );
            return time.ToString( "hh:mm tt" );
        }
    }
    public string currentTime24 {
        get {
            var timeSpan = System.TimeSpan.FromDays( m_dayPercent );
            System.DateTime time = System.DateTime.Today.Add( timeSpan );
            return time.ToString( "HH:MM" );
        }
    }
    private float m_timeElapsed = 0.0f;

    private void Start() {
        if( m_darkestTimeHours >= m_brightestTimeHours ) {
            throw new UnityException( "Darkest time must be BEFORE brightest time" );
        }

        // to check whether it's brightening or darkening
        m_darkestTime = m_darkestTimeHours / 24.0f * m_wholeDayLengthSeconds;
        m_brightestTime = m_brightestTimeHours / 24.0f * m_wholeDayLengthSeconds;
        Debug.Log( "Darkest at " + m_darkestTimeHours + "/" + m_darkestTime + ", brightest at " + m_brightestTimeHours + "/" + m_brightestTime );

        // day is easy
        m_brightLength = m_brightestTime - m_darkestTime;

        // night is time from sunset to end of day plus time until sunrise
        m_darkLength = m_wholeDayLengthSeconds - m_brightestTime + m_darkestTime;

        m_timeElapsed = m_startTime / 24.0f * m_wholeDayLengthSeconds;

        Debug.Log( "Day length: " + m_brightLength + ", night length: " + m_darkLength );
    }

    void Update () {
        var skyColor = m_dayColor;

        m_timeElapsed = Mathf.Repeat( m_timeElapsed + Time.deltaTime, m_wholeDayLengthSeconds );

        var t = 0.0f;
        if ( isBrightening ) {

            // brightening
            var timeSinceDarkest = m_timeElapsed - m_darkestTime;
            t = timeSinceDarkest / m_brightLength;
            skyColor = Color.Lerp( m_nightColor, m_dayColor, t );
            RenderSettings.fogDensity = Mathf.Lerp( m_fogMax, m_fogMin, t );
        } else {
            var timeSinceBrightest = 0.0f;

            if ( m_timeElapsed > m_brightestTime ) {

                // night time before looping
                timeSinceBrightest = m_timeElapsed - m_brightestTime;
            } else {
                
                // night time after looping
                timeSinceBrightest = m_wholeDayLengthSeconds - m_brightestTime + m_timeElapsed;
            }
            t = timeSinceBrightest / m_darkLength;
            skyColor = Color.Lerp( m_dayColor, m_nightColor, t );
            RenderSettings.fogDensity = Mathf.Lerp( m_fogMin, m_fogMax, t );
        }

        RenderSettings.fogColor = skyColor;
        Camera.main.backgroundColor = skyColor;
	}
}
