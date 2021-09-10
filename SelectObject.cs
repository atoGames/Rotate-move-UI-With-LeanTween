using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (GridLayoutGroup))]
public class SelectObject : MonoBehaviour {

    // index : to know what object stop on | selected 
    public int m_Index;
    // for Animation ( scale down/up)
    public List<Transform> m_Objects = new List<Transform> ();
    [Multiline (6)]
    public string[] m_Describe = new string[4] { "bad one \nis not fast \nSpeed : 5 \nHealth : 60 \nDamage : 16", "bla", "bla", "bla bla" };

    private GridLayoutGroup gridLayoutGroup;

    public bool isHorizontal = true; //Horizontal
    public bool isBackground = false; //Horizontal
    public float m_AnimationSpeed = 0.1f;
    private Vector3 m_ScaleUp = Vector3.one;
    private Vector3 m_ScaleDown = Vector3.one / 2;

    public GameObject txtDescribePrefab;

    public GameObject btnLeft, btnRigth;

    private void Start () {
        // if Objects = 0  : do this
        if (m_Objects.Count <= 0) {
            for (var i = 0; i < transform.childCount; i++) {
                m_Objects.Add (transform.GetChild (i));
            }
        }

        // Get grid layout group 
        gridLayoutGroup = null ?? GetComponent<GridLayoutGroup> ();

        // Horizontal ?
        if (isHorizontal) {
            gridLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = m_Objects.Count;
        } else {
            gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            gridLayoutGroup.constraintCount = 1;
        }
        // Background ?
        if (isBackground) {
            gridLayoutGroup.spacing = Vector2.zero;
            m_ScaleDown = Vector3.one;
        }

        // scale , Set Text and buttons
        foreach (var objs in m_Objects) {
            if (objs.transform.Find ("Back") != null && objs.transform.Find ("Front") != null) {
                Instantiate (txtDescribePrefab, objs.transform.Find ("Back"));
                objs.transform.Find ("Back").gameObject.AddComponent<Mask> ();

                objs.transform.Find ("Front").GetComponent<Button> ().onClick.AddListener (() => Rotate ());
                objs.transform.Find ("Back").GetComponent<Button> ().onClick.AddListener (() => Rotate ());
            }
            objs.localScale = m_ScaleDown;

        }

        // Change the size of the selected object
        m_Objects[m_Index].localScale = m_ScaleUp;

        ShowButtonsLeftAndRigth ();
    }

    public void MoveRigth () {
        // NEXT : Index + 1
        m_Index++;
        // Scale Down Previous Character / choose + make this line down of move if you don't like shake
        Scale (m_Objects[m_Index - 1].gameObject, m_ScaleDown);
        // Clamp Index
        ClampIndex ();
        // Move right
        Move (transform, m_Objects[m_Index % m_Objects.Count].gameObject, m_Index, isHorizontal);
    }
    public void MoveLeft () {
        // PREVIOUS : Index - 1
        m_Index--;
        // Scale Down Previous Character / choose + make this line down of move if you don't like shake
        Scale (m_Objects[m_Index + 1].gameObject, m_ScaleDown);
        // Clamp Index
        ClampIndex ();
        // Move left
        Move (transform, m_Objects[m_Index % m_Objects.Count].gameObject, m_Index, isHorizontal);
    }

    private void Move (Transform goMine, GameObject goSelect, int selectIndex, bool isHorizontal) {
        // for performance Disable it
        if (gridLayoutGroup.enabled)
            gridLayoutGroup.enabled = false;

        if (isHorizontal) {
            // Move to next or previous 
            LeanTween.moveLocalX (goMine.gameObject, selectIndex * -(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x), m_AnimationSpeed);
        } else
            LeanTween.moveLocalY (goMine.gameObject, selectIndex * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y), m_AnimationSpeed);
        // Scale Up Current Character / choose
        Scale (goSelect, m_ScaleUp);

        ShowButtonsLeftAndRigth ();
    }
    public void Rotate () {
        m_Objects[m_Index].transform.Find ("Back").GetChild (0).GetComponent<TextMeshProUGUI> ().text = m_Describe[m_Index];
        var y = m_Objects[m_Index].localRotation.y == 0 ? 170f : 0f;
        LeanTween.rotateY (m_Objects[m_Index].gameObject, y, m_AnimationSpeed).setOnStart (delegate { EnableOrDisable_Mask (y >= 0); });
    }
    //  Scale Down or Up  
    private void Scale (GameObject goSelect, Vector3 scale) => LeanTween.scale (goSelect, scale, m_AnimationSpeed);
    private void ClampIndex () => m_Index = Mathf.Clamp (m_Index, 0, m_Objects.Count - 1);

    private void ShowButtonsLeftAndRigth () {
        if (btnLeft == null || btnRigth == null)
            return;
        btnLeft.SetActive (m_Index > 0);
        btnRigth.SetActive (m_Index < m_Objects.Count - 1);
    }
    private void EnableOrDisable_Mask (bool ed) {
        if (m_Objects[m_Index].transform.Find ("Back") != null)
            m_Objects[m_Index].transform.Find ("Back").GetComponent<Mask> ().enabled = ed;

    }
    private bool isBackNull () {
        return m_Objects[m_Index].transform.Find ("Back");
    }
}