using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTween : MonoBehaviour
{
    #region Constants and Fields
    NavMeshAgent m_navAgent;
    CharacterController m_charCtr;
    [SerializeField]
    AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    Vector3 m_from;
    [SerializeField]
    Vector3 m_to;
    [SerializeField]
    float m_duration = 1f;
    float m_time;
    IEnumerator Coroutine_MoveCurve()
    {
        while(true)
        {
            m_time += Time.deltaTime / m_duration;
            var value = m_curve.Evaluate(m_time);
            var result = m_from * (1 - value) + m_to * value;
            m_charCtr.Move(result - transform.position);
            if (m_time > 1f)
            {
                yield return null;
            }
        }
    }

    #endregion

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine("Coroutine_MoveCurve");
    }
    public void Play(Vector3 from, Vector3 to, float duration)
    {
        m_from = from;
        m_to = to;
        m_duration = duration;
        StopAllCoroutines();
        StartCoroutine("Coroutine_MoveCurve");
    }


    void Start()
    {
        m_charCtr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Play();
        if(Input.GetKeyDown(KeyCode.O))
        {
            m_from = transform.position;
            m_to = m_from + Vector3.forward * 8f;
            m_duration = 0.5f;
            Play();
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            m_from = transform.position;
            m_to = m_from + Vector3.back * 8f;
            m_duration = 0.5f;
            Play();
        }
    }

}
