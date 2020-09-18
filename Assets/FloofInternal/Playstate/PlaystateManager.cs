#define FLOOF_PLAYSTATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlaystateManager : MonoBehaviour
{
    public RectAnimation EnterAnimation = default;
    public RectAnimation ExitAnimation = default;
    public EnumStates CurrentState = default;
    public List<PlayState> States = new List<PlayState>();

    GameObject Canvas;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
        for (int i = 0; i < States.Count; i++)
        {
            if (States[i].View.scene.rootCount != 0)
            {
                States[i].View.SetActive(false);
            }
            else
            {
                States[i].View = Instantiate(States[i].View, Canvas.transform);
                States[i].View.SetActive(false);
            }
        }
        SetState(CurrentState);
    }
    public PlayState GetState() => GetState(CurrentState);
    public PlayState GetState(EnumStates state)
    {
        for (int i = 0; i < States.Count; i++)
        {
            if (States[i].Name == state.ToString())
                return States[i];
        }
        Debug.LogError("State Dosen't exist");
        return null;
    }

    #region StateHandeling
    public void AddPlaystate(string stateName)
    {
        States.Add(new PlayState(stateName));
        generateStateEnum();
    }
    public void RemovePlaystate(EnumStates stateToRemove)
    {
        States.Remove(GetState(stateToRemove));
        generateStateEnum();
    }
    void generateStateEnum()
    {
        #if UNITY_EDITOR
        string[] stateNames = new string[States.Count];
        for (int i = 0; i < States.Count; i++)
            stateNames[i] = States[i].Name;
        EnumGenerator.Generate("EnumStates", stateNames);
        #endif
    }
    #endregion
    public void SetState(EnumStates state)
    {
        if (CurrentState != state)
        {
            StartCoroutine(Closing(CurrentState, true));
            GetState().StateExit_Start?.Invoke();
            CurrentState = state;
        }
        GetState().StateEnter_Start?.Invoke();
        StartCoroutine(Opening(CurrentState,true));
    }
    public void ChangeState(EnumStates newState)
    {
        if (CurrentState != newState)
        {
            StartCoroutine(Closing(CurrentState));
            GetState().StateExit_Start?.Invoke();
            CurrentState = newState;
            GetState().StateEnter_Start?.Invoke();
            StartCoroutine(Opening(CurrentState));
        }
        else
        {
            StartCoroutine(Opening(CurrentState, true));
        }
    }
    IEnumerator Opening(EnumStates state, bool instant = false)
    {
        RectTransform rect = GetState(state).View.GetComponent<RectTransform>();
        if (!instant)
        {
            yield return new WaitForSeconds(EnterAnimation.Delay);
            rect.gameObject.SetActive(true);
            if (EnterAnimation.Move || EnterAnimation.Scale || EnterAnimation.Rotate)
            {
                Vector2 endPos = EnterAnimation.CustomPosition ? EnterAnimation.EndPosition : FindCanvasEdge(EnterAnimation.EndDir);
                Vector2 startPos = EnterAnimation.CustomPosition ? EnterAnimation.StartPosition : FindCanvasEdge(EnterAnimation.StartDir);

                for (float t = 0; t < EnterAnimation.Time; t += Time.deltaTime)
                {
                    if (EnterAnimation.Move)
                    {
                        float x = ((endPos.x - startPos.x) * (EnterAnimation.moveCurve.Evaluate(t / EnterAnimation.Time))) + startPos.x;
                        float y = ((endPos.y - startPos.y) * (EnterAnimation.moveCurve.Evaluate(t / EnterAnimation.Time))) + startPos.y;
                        rect.anchoredPosition = new Vector2(x, y);
                    }
                    else
                        rect.anchoredPosition = Vector2.zero;
                    if (EnterAnimation.Scale)
                    {
                        float x = ((EnterAnimation.EndScale.x - EnterAnimation.StartScale.x) * (EnterAnimation.ScaleCurve.Evaluate(t / EnterAnimation.Time))) + EnterAnimation.StartScale.x;
                        float y = ((EnterAnimation.EndScale.y - EnterAnimation.StartScale.y) * (EnterAnimation.ScaleCurve.Evaluate(t / EnterAnimation.Time))) + EnterAnimation.StartScale.y;
                        rect.localScale = new Vector2(x, y);
                    }
                    else
                        rect.localScale = Vector2.one;
                    if (EnterAnimation.Rotate)
                    {
                        float r = (EnterAnimation.startRotation - EnterAnimation.EndRotation) * (1 - EnterAnimation.RotationCurve.Evaluate(t / EnterAnimation.Time));
                        rect.localRotation = Quaternion.Euler(0, 0, r);
                    }
                    else
                        rect.localRotation = Quaternion.Euler(0, 0, 0);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        rect.anchoredPosition = EnterAnimation.EndPosition;
        rect.localScale = EnterAnimation.EndScale;
        rect.localRotation = Quaternion.Euler(0, 0, EnterAnimation.EndRotation);
        GetState(state).StateEnter_End?.Invoke();
    }
    IEnumerator Closing(EnumStates state,bool instant = false)
    {
            RectTransform rect = GetState(state).View.GetComponent<RectTransform>();
        if (!instant)
        {
            yield return new WaitForSeconds(ExitAnimation.Delay);
            if (ExitAnimation.Move || ExitAnimation.Scale || ExitAnimation.Rotate)
            {
                Vector2 endPos = ExitAnimation.CustomPosition ? ExitAnimation.EndPosition : FindCanvasEdge(ExitAnimation.EndDir);
                Vector2 startPos = ExitAnimation.CustomPosition ? ExitAnimation.StartPosition : FindCanvasEdge(ExitAnimation.StartDir);

                for (float t = 0; t < ExitAnimation.Time; t += Time.deltaTime)
                {
                    if (ExitAnimation.Move)
                    {

                        float x = ((endPos.x - startPos.x) * (ExitAnimation.moveCurve.Evaluate(t / ExitAnimation.Time))) + startPos.x;
                        float y = ((endPos.y - startPos.y) * (ExitAnimation.moveCurve.Evaluate(t / ExitAnimation.Time))) + startPos.y;
                        rect.anchoredPosition = new Vector2(x, y);
                    }
                    else
                        rect.anchoredPosition = Vector2.zero;
                    if (ExitAnimation.Scale)
                    {
                        float x = ((ExitAnimation.EndScale.x - ExitAnimation.StartScale.x) * (ExitAnimation.ScaleCurve.Evaluate(t / ExitAnimation.Time))) + ExitAnimation.StartScale.x;
                        float y = ((ExitAnimation.EndScale.y - ExitAnimation.StartScale.y) * (ExitAnimation.ScaleCurve.Evaluate(t / ExitAnimation.Time))) + ExitAnimation.StartScale.y;
                        rect.localScale = new Vector2(x, y);
                    }
                    else
                        rect.localScale = Vector2.one;
                    if (ExitAnimation.Rotate)
                    {
                        float r = (ExitAnimation.EndRotation - ExitAnimation.startRotation) * ExitAnimation.RotationCurve.Evaluate(t / ExitAnimation.Time);
                        rect.localRotation = Quaternion.Euler(0, 0, r);
                    }
                    else
                        rect.localRotation = Quaternion.Euler(0, 0, 0);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        rect.anchoredPosition = ExitAnimation.EndPosition;
        rect.localScale = ExitAnimation.EndScale;
        rect.localRotation = Quaternion.Euler(0,0,ExitAnimation.EndRotation);
        rect.gameObject.SetActive(false);
        GetState(state).StateExit_End?.Invoke();
    }
    Vector2 FindCanvasEdge(direction dir)
    {
        float x = Canvas.GetComponent<RectTransform>().sizeDelta.x;
        float y = Canvas.GetComponent<RectTransform>().sizeDelta.y;
        switch (dir)
        {
            case direction.Center:
                return Vector2.zero;
            case direction.Up:
                return new Vector2(0, y);
            case direction.UpLeft:
                return new Vector2(-x, y);
            case direction.UpRight:
                return new Vector2(x, y);
            case direction.Left:
                return new Vector2(-x, 0);
            case direction.Right:
                return new Vector2(x, 0);
            case direction.Down:
                return new Vector2(0, -y);
            case direction.DownLeft:
                return new Vector2(-x, -y);
            case direction.DownRight:
                return new Vector2(x, -y);
            default: 
                return Vector3.zero;
        }
    }

}
[System.Serializable]
public class RectAnimation : object
{
    public float Delay = 0f;
    public float Time = 1f;
    [Space]
    public bool Move = default;
    public direction StartDir = default;
    public direction EndDir = default;
    public bool CustomPosition = default;
    public Vector2 StartPosition = new Vector2(250, 0);
    public Vector2 EndPosition = Vector2.zero;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Space]
    public bool Scale = default;
    public Vector2 StartScale = Vector2.zero;
    public Vector2 EndScale = Vector2.one;
    public AnimationCurve ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Space]
    public bool Rotate = default;
    public float startRotation = 0;
    public float EndRotation = 0;
    public AnimationCurve RotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

}
public delegate void emptyDelegate();
[System.Serializable]
public class PlayState : object
{
    public string Name;

    public emptyDelegate StateEnter_Start;
    public emptyDelegate StateEnter_End;

    public emptyDelegate StateExit_Start;
    public emptyDelegate StateExit_End;

    public GameObject View;
    public PlayState(string name)
    {
        Name = name;
    }
}

public enum direction
{
    Center,
    Up,
    UpLeft,
    UpRight,
    Left,
    Right,
    Down,
    DownLeft,
    DownRight
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlaystateManager))]
public class PlayStatesEditor : Editor
{
    PlaystateManager scr;
    string nameToAdd;
    EnumStates stateToRemove;
    EnumStates stateToChangeTo;
    private void OnEnable()
    {
        scr = (PlaystateManager)target;
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        stateToChangeTo = (EnumStates)EditorGUILayout.EnumPopup(stateToChangeTo,GUILayout.Width(200));
        if (GUILayout.Button("Change State"))
            scr.ChangeState(stateToChangeTo);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        nameToAdd = EditorGUILayout.TextField(nameToAdd, GUILayout.Width(200));
        if (GUILayout.Button("Add State"))
        {
            string setName = nameToAdd.Replace(' ', '_');
            scr.AddPlaystate(setName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        stateToRemove = (EnumStates)EditorGUILayout.EnumPopup(stateToRemove, GUILayout.Width(200));
        if (GUILayout.Button("Remove State"))
        {
            scr.RemovePlaystate(stateToRemove);
        }
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
#endif