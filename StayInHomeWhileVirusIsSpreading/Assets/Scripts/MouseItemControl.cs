using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum MouseMode
{
    QItem,
    WItem,
    EItem,
    Normal,
    Whatever
}

public class MouseItemControl : MonoBehaviour
{
    public Texture2D QItem;
    public Texture2D QItemPressed;
    public Texture2D WItem;
    public Texture2D WItemPressed;
    public Texture2D EItem;
    public Texture2D EItemPressed;
    public Texture2D NormalCursor;
    
    public CursorMode GameCursorMode = CursorMode.Auto;
    public Vector2 HotSpot = Vector2.one * 0.5f;
    private Vector2 HotSpotLeftUp = Vector2.zero;
    private Texture2D m_unPressedTex;
    private Texture2D m_pressedTex;
    private bool m_mousePressed = false;
    private MouseMode m_currentCursorMode = MouseMode.Normal;
    public static MouseMode CurrentMouseMode;

    public static bool ModeUseful(MouseMode mode)
    {
        if (mode == MouseMode.Whatever)
        {
            return true;
        }
        return CurrentMouseMode == mode;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_currentCursorMode = MouseMode.Normal;
        UpdateCursorTex();
        m_pressedTex = NormalCursor;
    }

    void UpdateCursorTex()
    {
        switch (m_currentCursorMode)
        {
           case MouseMode.Normal:
               Cursor.SetCursor(NormalCursor,HotSpotLeftUp,GameCursorMode);
               break;
           case MouseMode.QItem:
               Cursor.SetCursor(QItem,HotSpot,GameCursorMode);
                break;
           case MouseMode.WItem:
               Cursor.SetCursor(WItem,HotSpot,GameCursorMode);
                break;
           case MouseMode.EItem:
               Cursor.SetCursor(EItem,HotSpot,GameCursorMode);
                break;
           default:
               Cursor.SetCursor(NormalCursor,HotSpot,GameCursorMode);
               break;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_pressedTex = QItemPressed;
            m_currentCursorMode = MouseMode.QItem;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            m_pressedTex = WItemPressed;
            m_currentCursorMode = MouseMode.WItem;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_pressedTex = EItemPressed;
            m_currentCursorMode = MouseMode.EItem;
        }

        CurrentMouseMode = m_currentCursorMode;

        if (Input.GetMouseButtonDown(0))
        {
            m_mousePressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_mousePressed = false;
        }

        if (m_mousePressed)
        {
            Cursor.SetCursor(m_pressedTex,HotSpot,GameCursorMode);
        }
        else
        {
            UpdateCursorTex();
        }

        if (!Gamemanager.Instance.PlayMode)
        {
            m_currentCursorMode = MouseMode.Normal;
            Cursor.SetCursor(NormalCursor,HotSpotLeftUp,GameCursorMode);
        }
    }
}
