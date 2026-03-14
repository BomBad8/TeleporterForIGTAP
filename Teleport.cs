using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

[BepInPlugin("com.you.teleportmod", "TeleportMod", "1.0.0")]
public class TeleportMod : BaseUnityPlugin
{
    private ManualLogSource _log;
    private bool _menuOpen = false;

    private readonly (string icon, string label, float x, float y)[] _locations = new[]
    {
        ("1", "Course 1", 2241.0f,   -292.0f),
        ("2", "Course 2", 4783.43f,  1876.22f),
        ("3", "Course 3", 9506.0f,   6971.0f),
        ("4", "Course 4", 18416.55f, 10556.0f),
        ("5", "Course 5", -3282.0f,  -294.0f),
    };

    private static readonly Color BG         = new Color(0.22f, 0.22f, 0.23f, 0.98f);
    private static readonly Color HEADER_BG  = new Color(0.18f, 0.18f, 0.19f, 1f);
    private static readonly Color BTN_NORM   = new Color(0.26f, 0.26f, 0.27f, 1f);
    private static readonly Color BTN_HOVER  = new Color(0.92f, 0.45f, 0.08f, 1f);
    private static readonly Color ICON_BG    = new Color(0.30f, 0.30f, 0.32f, 1f);
    private static readonly Color ICON_HOVER = new Color(0.92f, 0.45f, 0.08f, 1f);
    private static readonly Color ACCENT     = new Color(1.00f, 0.60f, 0.05f, 1f);
    private static readonly Color BORDER     = new Color(0.35f, 0.35f, 0.37f, 1f);
    private static readonly Color TEXT_MAIN  = new Color(0.95f, 0.95f, 0.95f, 1f);
    private static readonly Color TEXT_DIM   = new Color(0.60f, 0.60f, 0.62f, 1f);
    private static readonly Color TEXT_ICON  = new Color(1.00f, 0.60f, 0.05f, 1f);
    private static readonly Color CLOSE_NORM = new Color(0.60f, 0.60f, 0.62f, 1f);
    private static readonly Color CLOSE_HOV  = new Color(1.00f, 0.60f, 0.05f, 1f);

    private Texture2D _texBG, _texHeader, _texBtn, _texBtnHover;
    private Texture2D _texIcon, _texIconHover, _texBorder, _texAccent, _texTransp;
    private GUIStyle _stylePanel, _styleHeader, _styleTitle;
    private GUIStyle _styleCloseNorm, _styleCloseHov;
    private GUIStyle _styleIcon, _styleIconHover, _styleLabel, _styleLabelHover;
    private GUIStyle _styleHint;

    void Awake()
    {
        _log = Logger;
        InitStyles();
        _log.LogInfo("TeleportMod loaded");
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.tKey.wasPressedThisFrame)
        {
            _menuOpen = !_menuOpen;
            _log.LogInfo($"Menu toggled: {_menuOpen}");
        }

        if (kb.escapeKey.wasPressedThisFrame && _menuOpen)
            _menuOpen = false;
    }

    Texture2D MakeTex(Color c)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    void InitStyles()
    {
        _texBG        = MakeTex(BG);
        _texHeader    = MakeTex(HEADER_BG);
        _texBtn       = MakeTex(BTN_NORM);
        _texBtnHover  = MakeTex(BTN_HOVER);
        _texIcon      = MakeTex(ICON_BG);
        _texIconHover = MakeTex(ICON_HOVER);
        _texBorder    = MakeTex(BORDER);
        _texAccent    = MakeTex(ACCENT);
        _texTransp    = MakeTex(new Color(0, 0, 0, 0));

        _stylePanel = new GUIStyle
        {
            normal = { background = _texBG }
        };

        _styleHeader = new GUIStyle
        {
            normal = { background = _texHeader }
        };

        _styleTitle = new GUIStyle
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            normal = { textColor = TEXT_DIM },
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(16, 0, 0, 0)
        };

        _styleCloseNorm = new GUIStyle
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { background = _texTransp, textColor = CLOSE_NORM }
        };

        _styleCloseHov = new GUIStyle(_styleCloseNorm)
        {
            normal = { background = _texTransp, textColor = CLOSE_HOV }
        };

        _styleIcon = new GUIStyle
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { background = _texIcon, textColor = TEXT_ICON }
        };

        _styleIconHover = new GUIStyle(_styleIcon)
        {
            normal = { background = _texIconHover, textColor = Color.white }
        };

        _styleLabel = new GUIStyle
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(14, 0, 0, 0),
            normal = { background = _texBtn, textColor = TEXT_MAIN }
        };

        _styleLabelHover = new GUIStyle(_styleLabel)
        {
            fontStyle = FontStyle.Bold,
            normal = { background = _texBtnHover, textColor = Color.white }
        };

        _styleHint = new GUIStyle
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = TEXT_DIM }
        };
    }

    void OnGUI()
    {
        if (!_menuOpen) return;

        float W       = 270f;
        float headerH = 38f;
        float iconW   = 42f;
        float rowH    = 46f;
        float gap     = 5f;
        float padV    = 10f;
        float hintH   = 26f;
        float totalH  = headerH + padV + (_locations.Length * (rowH + gap)) - gap + padV + hintH;
        float px      = Screen.width - W - 24f;
        float py      = (Screen.height - totalH) / 2f;

        // Panel
        GUI.Box(new Rect(px, py, W, totalH), GUIContent.none, _stylePanel);

        // Top accent line
        GUI.color = ACCENT;
        GUI.DrawTexture(new Rect(px, py, W, 3f), _texAccent);
        GUI.color = Color.white;

        // Header
        GUI.Box(new Rect(px, py + 3f, W, headerH), GUIContent.none, _styleHeader);
        GUI.Label(new Rect(px, py + 3f, W - 40f, headerH), "TELEPORT MENU", _styleTitle);

        // Close button
        Rect closeRect = new Rect(px + W - 34f, py + 8f, 22f, 22f);
        bool closeHov  = closeRect.Contains(Event.current.mousePosition);
        if (GUI.Button(closeRect, "✕", closeHov ? _styleCloseHov : _styleCloseNorm))
            _menuOpen = false;

        // Divider
        GUI.color = BORDER;
        GUI.DrawTexture(new Rect(px, py + 3f + headerH, W, 1f), _texBorder);
        GUI.color = Color.white;

        // Rows
        float curY = py + 3f + headerH + padV;
        for (int i = 0; i < _locations.Length; i++)
        {
            var loc      = _locations[i];
            float labelW = W - 10f - iconW - gap - 10f;

            Rect iconRect  = new Rect(px + 10f, curY, iconW, rowH);
            Rect labelRect = new Rect(px + 10f + iconW + gap, curY, labelW, rowH);

            bool anyHov = iconRect.Contains(Event.current.mousePosition)
                       || labelRect.Contains(Event.current.mousePosition);

            if (GUI.Button(iconRect,  loc.icon,  anyHov ? _styleIconHover  : _styleIcon))
                TeleportTo(loc.x, loc.y);

            if (GUI.Button(labelRect, loc.label, anyHov ? _styleLabelHover : _styleLabel))
                TeleportTo(loc.x, loc.y);

            curY += rowH + gap;
        }

        // Hint
        GUI.Label(new Rect(px, py + totalH - hintH, W, hintH), "[T] toggle  •  [ESC] close", _styleHint);
    }

    void TeleportTo(float x, float y)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            _log.LogWarning("Player not found — check tag");
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = new Vector3(x, y, player.transform.position.z);
            cc.enabled = true;
        }
        else
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
            player.transform.position = new Vector3(x, y, player.transform.position.z);
        }

        _log.LogInfo($"Teleported to {x}, {y}");
        _menuOpen = false;
    }
}