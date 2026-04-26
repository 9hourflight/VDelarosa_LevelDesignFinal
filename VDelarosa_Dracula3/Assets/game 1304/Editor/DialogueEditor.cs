using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public enum LinkMode { link,unlink,cancel,child, none};

public class DialogueEditor : EditorWindow
{
    DialogueScriptableObject currentDialogue;


    //GUI stuff
    [NonSerialized]
    bool isDragging = false;
    [NonSerialized]
    bool showCreationMenu = false;
    [NonSerialized]
    Vector2 creationMenuPosition;
    DialogueNodeBase GUIselectedNode;
    Vector2 scrollPosition;
    Vector2 mouseOffset = Vector2.zero;
    [NonSerialized]
    bool isDraggingScreen = false;
    Vector2 drawAreaOffset = Vector2.zero;
    Vector2 screenDragStart = Vector2.zero;
    [NonSerialized]
    DialogueNodeBase creatingNode = null;
    [NonSerialized]
    Type nodeTypeToCreate;
    [NonSerialized]
    bool isCreatingNode = false;
    [NonSerialized]
    DialogueNodeBase deletingNode = null;
    [NonSerialized]
    DialogueNodeBase linkingNode = null;
    

    const float canvasSize = 4000f;
    const float backgroundSize = 64f;

    public GUIStyle createMenuNodeStyle;
    GUIStyle activeDebugNodeStyle;

    [MenuItem("Window/GAME 1304/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
    }

    

    [OnOpenAsset(1)]
    public static bool OpenDialogueAsset(int instID, int lineNum)
    {
        DialogueScriptableObject dialogue;
        dialogue = EditorUtility.InstanceIDToObject(instID) as DialogueScriptableObject;
        if (dialogue != null)
        {            
            ShowEditorWindow();
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        //registering an event callback
        Selection.selectionChanged += OnSelectionChanged;


        createMenuNodeStyle = new GUIStyle();
        createMenuNodeStyle.normal.background = EditorGUIUtility.Load("Assets/game 1304/Editor/Resources/nodeBackYellow.png") as Texture2D;
        createMenuNodeStyle.normal.textColor = Color.white;
        createMenuNodeStyle.padding = new RectOffset(10, 10, 10, 10);
        createMenuNodeStyle.border = new RectOffset(12, 12, 12, 12);

        activeDebugNodeStyle = new GUIStyle();
        activeDebugNodeStyle.normal.background = EditorGUIUtility.Load("Assets/game 1304/Editor/Resources/nodeBackRed.png") as Texture2D;
        activeDebugNodeStyle.normal.textColor = Color.red;
        activeDebugNodeStyle.padding = new RectOffset(10, 10, 10, 10);
        activeDebugNodeStyle.border = new RectOffset(12, 12, 12, 12);

    }

    private void OnSelectionChanged()
    {
        DialogueScriptableObject dialogue;
        dialogue =Selection.activeObject as DialogueScriptableObject;
        if(dialogue != null)
        {
            currentDialogue = dialogue;
            Repaint();
        }
    }

    private void OnGUI()
    {
        if (currentDialogue != null)
        {
            ProcessEvents();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true); // GUILayout.Width(500),GUILayout.Height(500));
            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
            Texture2D backTex = Resources.Load("gridBack") as Texture2D;

            //GUI.DrawTexture(canvas, backTex);
            GUI.DrawTextureWithTexCoords(canvas, backTex, new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize) );
            foreach (DialogueNodeBase node in currentDialogue.GetAllNodes())
            {             
                DrawConnections(node);
            }
            foreach (DialogueNodeBase node in currentDialogue.GetAllNodes())
            {
                DrawNode(node);             
            }

            if(showCreationMenu)
            {
                DrawCreateMenu();
            }
            GUILayout.EndScrollView();

            //TODO: work this into the creation menu
            if(isCreatingNode)
            {

                currentDialogue.CreateChildNode(creatingNode, nodeTypeToCreate, scrollPosition+ creationMenuPosition);
                creatingNode = null;
                isCreatingNode = false;
            }
            if (deletingNode != null)
            {
                
                currentDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }
        else
        {
            EditorGUILayout.LabelField("No dialogue selected");
        }
        
    }

    private void DrawConnections(DialogueNodeBase node)
    {
        foreach (DialogueNodeBase childNode in currentDialogue.GetAllChildren(node))
        {
            Vector3 startPos = new Vector3(node.GUIRect.xMax,node.GUIRect.center.y,0);
            Vector3 startMid = new Vector3(Mathf.Lerp(node.GUIRect.xMax,childNode.GUIRect.xMin,0.45f), node.GUIRect.center.y, 0);
            Vector3 endPos = new Vector3(childNode.GUIRect.xMin, childNode.GUIRect.center.y, 0);
            Vector3 endMid = new Vector3(Mathf.Lerp(node.GUIRect.xMax, childNode.GUIRect.xMin, 0.65f), childNode.GUIRect.center.y, 0);
            Handles.DrawBezier(startPos, endPos, startMid, endMid, Color.white, null, 4f);
        }
    }

    private void ProcessEvents()
    {
        switch (Event.current.type)
        {            
            case EventType.MouseDown:
                //if ((!isDragging)&&(!isDraggingScreen))
                //{
                    GUIselectedNode = null;
                    foreach (DialogueNodeBase node in currentDialogue.GetAllNodes())
                    {
                        
                        if (node.GUIRect.Contains(Event.current.mousePosition+ scrollPosition))
                        {
                            isDragging = true;                            
                            GUIselectedNode = node;
                        mouseOffset = node.GUIRect.position - Event.current.mousePosition; // + scrollPosition;
                        }
                    }
                    if (GUIselectedNode == null)
                    {
                    if (Event.current.button == 1)
                    {
                        showCreationMenu = true;
                        creationMenuPosition = Event.current.mousePosition; // + scrollPosition;
                        creatingNode = null;
                    }
                    else
                    {
                        isDraggingScreen = true;
                        screenDragStart = Event.current.mousePosition + scrollPosition;
                    }
                    Selection.activeObject = currentDialogue;
                    }
                    else
                    {
                        Selection.activeObject = GUIselectedNode;
                    }
                //}
                
                break;
            case EventType.MouseDrag:
                if(isDragging)
                {
                    if (GUIselectedNode != null)
                    {                        
                        GUIselectedNode.SetPosition(Event.current.mousePosition+mouseOffset);                        
                        GUI.changed = true;
                    }
                }
                if(isDraggingScreen)
                {
                    scrollPosition = screenDragStart - Event.current.mousePosition;
                    //drawAreaOffset = screenDragStart - Event.current.mousePosition;
                    GUI.changed = true;
                }
                break;
            case EventType.MouseUp:
                if (Event.current.button == 0)
                {
                    if (isDragging)
                        isDragging = false;
                    if (isDraggingScreen)
                        isDraggingScreen = false;
                }
                break;
        }
        
    }
    private void DrawNode(DialogueNodeBase node)
    {
        
        

        if (node as DialogueNode_NPCEntry)
        {
            DrawNode_NPCEntry(node as DialogueNode_NPCEntry);
        }
        if (node as DialogueNode_PlayerEntry)
        {
            DrawNode_PlayerEntry(node as DialogueNode_PlayerEntry);
        }
        if (node as DialogueNode_Line)
        {
            DrawNode_Line(node as DialogueNode_Line);
        }
        if (node as DialogueNode_Reply)
        {
            DrawNode_Reply(node as DialogueNode_Reply);
        }
        if (node as DialogueNode_LogicToken)
        {
            DrawNode_LogicToken(node as DialogueNode_LogicToken);
        }
        if (node as DialogueNode_LogicQuest)
        {
            DrawNode_LogicQuest(node as DialogueNode_LogicQuest);
        }
        if (node as DialogueNode_LogicInventory)
        {
            DrawNode_LogicInventory(node as DialogueNode_LogicInventory);
        }
        
    }

    private void DrawNode_LogicInventory(DialogueNode_LogicInventory node)
    {
        LinkMode linkMode = LinkMode.none;
        string modeLabel = "";

        if (node.parentEntryMode == DialogueEntryMode.NPCEntry)
            modeLabel = "(NPC)";
        if (node.parentEntryMode == DialogueEntryMode.playerEntry)
            modeLabel = "(Player)";
        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("X"))
                {
                    deletingNode = node;
                }
                EditorGUILayout.LabelField("Logic:Inv" + modeLabel, EditorStyles.whiteLabel);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {

                if (GUILayout.Button("+"))
                {
                    creatingNode = node;
                    showCreationMenu = true;
                    creationMenuPosition = Event.current.mousePosition + scrollPosition;
                }

                linkMode = GetLogicNodeLinkMode(node);
                ShowLinkButton(linkMode, node);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

    }

    private void DrawNode_LogicQuest(DialogueNode_LogicQuest node)
    {
        LinkMode linkMode = LinkMode.none;
        string modeLabel = "";

        if (node.parentEntryMode == DialogueEntryMode.NPCEntry)
            modeLabel = "(NPC)";
        if (node.parentEntryMode == DialogueEntryMode.playerEntry)
            modeLabel = "(Player)";
        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("X"))
                {
                    deletingNode = node;
                }
                EditorGUILayout.LabelField("Logic:Quest" + modeLabel, EditorStyles.whiteLabel);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("+"))
                    {
                        creatingNode = node;
                        showCreationMenu = true;
                        creationMenuPosition = Event.current.mousePosition + scrollPosition;
                    }

                    linkMode = GetLogicNodeLinkMode(node);
                    ShowLinkButton(linkMode, node);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

    }

    private void DrawNode_LogicToken(DialogueNode_LogicToken node)
    {
        LinkMode linkMode = LinkMode.cancel;
        string modeLabel = "";

        if (node.parentEntryMode == DialogueEntryMode.NPCEntry)
            modeLabel = "(NPC)";
        if (node.parentEntryMode == DialogueEntryMode.playerEntry)
            modeLabel = "(Player)";
        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("X"))
                {
                    deletingNode = node;
                }
                EditorGUILayout.LabelField("Logic:Token"+modeLabel, EditorStyles.whiteLabel);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Token name:", EditorStyles.whiteLabel);
            node.setTokenName(EditorGUILayout.TextField(node.tokenName));
            GUILayout.BeginHorizontal();
            {
                //EditorGUILayout.LabelField("Possible Lines", EditorStyles.whiteLabel);

                if (GUILayout.Button("+"))
                {
                    creatingNode = node;
                    showCreationMenu = true;
                    creationMenuPosition = Event.current.mousePosition + scrollPosition;
                }
                
                linkMode = GetLogicNodeLinkMode(node);
                ShowLinkButton(linkMode, node);

            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private LinkMode GetLogicNodeLinkMode(DialogueNode_Logic node)
    {
        LinkMode linkMode;
        if (linkingNode != null)
        {
            linkMode = LinkMode.none;
            if (node == linkingNode)
                linkMode = LinkMode.cancel;
            else
            {
                if (linkingNode.GetChildren().Contains(node.entryAddress))
                {
                    linkMode = LinkMode.unlink;
                }
                else
                {
                    if (linkingNode as DialogueNode_Logic != null)
                    {
                        if (((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none) ||
                                ((linkingNode as DialogueNode_Logic).parentEntryMode == node.parentEntryMode) ||
                                (node.parentEntryMode == DialogueEntryMode.none))
                        {
                            linkMode = LinkMode.child;
                        }
                    }
                    if (linkingNode as DialogueNode_NPCEntry != null)
                    {
                        if ((node.parentEntryMode == DialogueEntryMode.none) || (node.parentEntryMode == DialogueEntryMode.NPCEntry))
                        {
                            linkMode = LinkMode.child;
                        }
                    }
                    if (linkingNode as DialogueNode_PlayerEntry != null)
                    {
                        if ((node.parentEntryMode == DialogueEntryMode.none) || (node.parentEntryMode == DialogueEntryMode.playerEntry))
                        {
                            linkMode = LinkMode.child;
                        }
                    }

                }
            }
        }
        else
        {
            linkMode = LinkMode.link;
        }
        return linkMode;
    }
    private void DrawNode_Reply(DialogueNode_Reply node)
    {
        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            LinkMode linkMode = LinkMode.link;

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("X"))
                {
                    deletingNode = node;
                }
                EditorGUILayout.LabelField("Dialogue Reply", EditorStyles.whiteLabel);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Reply:", EditorStyles.whiteLabel);
                node.setReply(EditorGUILayout.TextField(node.reply));
                GUILayout.BeginHorizontal();
                {
                    if (node.GetChildren().Count == 0)
                    {
                        if (GUILayout.Button("+"))
                        {
                            creatingNode = node;
                            showCreationMenu = true;
                            creationMenuPosition = Event.current.mousePosition + scrollPosition;
                        }
                    }

                    if (linkingNode != null)
                    {
                        linkMode = LinkMode.none;
                        if (node == linkingNode)
                            linkMode = LinkMode.cancel;
                        else
                        {
                            if (linkingNode.GetChildren().Contains(node.entryAddress))
                            {
                                linkMode = LinkMode.unlink;
                            }
                            else
                            {
                                if(linkingNode as DialogueNode_PlayerEntry != null)
                                    linkMode = LinkMode.child;
                                if(linkingNode as DialogueNode_Logic != null)
                                {
                                    if(((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.playerEntry) ||
                                        ((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none))
                                    {
                                        linkMode = LinkMode.child;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        linkMode = LinkMode.link;
                    }

                    ShowLinkButton(linkMode, node);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }

    private void DrawNode_Line(DialogueNode_Line node)
    {
        LinkMode linkMode = LinkMode.link;

        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("X"))
                {
                    deletingNode = node;
                }
                EditorGUILayout.LabelField("NPC Dialogue Line", EditorStyles.whiteLabel);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Line:", EditorStyles.whiteLabel);
            node.setLineText(EditorGUILayout.TextField(node.lineText));

            if (linkingNode != null)
            {
                linkMode = LinkMode.none;
                if (node == linkingNode)
                    linkMode = LinkMode.cancel;
                else
                {
                    if (linkingNode.GetChildren().Contains(node.entryAddress))
                    {
                        linkMode = LinkMode.unlink;
                    }
                    else
                    {
                        if (linkingNode as DialogueNode_NPCEntry != null)
                            linkMode = LinkMode.child;
                        if (linkingNode as DialogueNode_Logic != null)
                        {
                            if (((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.NPCEntry) ||
                                ((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none))
                            {
                                linkMode = LinkMode.child;
                            }
                        }
                    }
                }
            }
            else
            {
                linkMode = LinkMode.none;
            }

            ShowLinkButton(linkMode, node);
        }
        GUILayout.EndArea();
    }

    private void DrawNode_NPCEntry(DialogueNode_NPCEntry node)
    {
        LinkMode linkMode = LinkMode.cancel;

        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    if (node != currentDialogue.GetRoot())
                    {
                        if (GUILayout.Button("X"))
                        {
                            deletingNode = node;
                        }
                    }
                    EditorGUILayout.LabelField("NPC Dialogue Tree"+((node==currentDialogue.GetRoot())?" - ROOT":""), EditorStyles.whiteLabel);
                    EditorGUILayout.Space(10);
                    
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {

                    if (GUILayout.Button("+"))
                    {
                        creatingNode = node;
                        showCreationMenu = true;
                        creationMenuPosition = Event.current.mousePosition + scrollPosition;

                    }
                    if (linkingNode != null)
                    {
                        
                        linkMode = LinkMode.none;

                        if (node == linkingNode)
                            linkMode = LinkMode.cancel;
                        else
                        {
                            if (linkingNode.GetChildren().Contains(node.entryAddress))
                            {
                                linkMode = LinkMode.unlink;
                            }
                            else
                            {
                                //if we're linking from a reply, a reply can only have one entry coming off of it
                                if (linkingNode as DialogueNode_Reply != null)
                                {
                                    if (linkingNode.GetChildren().Count == 0)
                                        linkMode = LinkMode.child;
                                    else
                                        linkMode = LinkMode.none;
                                }
                                
                                //if we're linking from another NPC entry, this one can be the only NPC entry child
                                if (linkingNode as DialogueNode_NPCEntry != null)
                                {
                                    linkMode = LinkMode.child;
                                    foreach (string s in linkingNode.GetChildren())
                                    {
                                        if ((currentDialogue.GetEntryByAddress(s) as DialogueNode_PlayerEntry) ||
                                            (currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry))
                                            linkMode = LinkMode.none;
                                    }
                                }


                            }
                        }
                    }
                    else
                    {
                        linkMode = LinkMode.link;
                    }

                    ShowLinkButton(linkMode, node);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }

    private void DrawNode_PlayerEntry(DialogueNode_PlayerEntry node)
    {
        LinkMode linkMode = LinkMode.cancel;

        GUIStyle tempGS;
        if (node.entryAddress == DialogueManager.activeEntryAddress)
        {
            tempGS = activeDebugNodeStyle;
        }
        else
            tempGS = node.nodeStyle;
        GUILayout.BeginArea(node.GUIRect, tempGS);
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    if (node != currentDialogue.GetRoot())
                    {
                        if (GUILayout.Button("X"))
                        {
                            deletingNode = node;
                        }
                    }
                    EditorGUILayout.LabelField("Player Dialogue Tree" + ((node == currentDialogue.GetRoot()) ? " - ROOT" : ""), EditorStyles.whiteLabel);
                    EditorGUILayout.Space(10);

                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {

                    if (GUILayout.Button("+"))
                    {
                        creatingNode = node;
                        showCreationMenu = true;
                        creationMenuPosition = Event.current.mousePosition + scrollPosition;

                    }
                    if (linkingNode != null)
                    {

                        linkMode = LinkMode.none;

                        if (node == linkingNode)
                            linkMode = LinkMode.cancel;
                        else
                        {
                            if (linkingNode.GetChildren().Contains(node.entryAddress))
                            {
                                linkMode = LinkMode.unlink;
                            }
                            else
                            {                                
                                //if we're linking from an NPC entry, this one can be the only entry child
                                if (linkingNode as DialogueNode_NPCEntry != null)
                                {
                                    linkMode = LinkMode.child;
                                    foreach (string s in linkingNode.GetChildren())
                                    {
                                        if ((currentDialogue.GetEntryByAddress(s) as DialogueNode_PlayerEntry)||
                                            (currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry))
                                            linkMode = LinkMode.none;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        linkMode = LinkMode.link;
                    }

                    ShowLinkButton(linkMode, node);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }

    void ShowLinkButton(LinkMode linkMode, DialogueNodeBase node)
    {
        switch (linkMode)
        {
            case LinkMode.link:
                if (GUILayout.Button("Link"))
                {
                    linkingNode = node;
                }
                break;
            case LinkMode.unlink:
                if (GUILayout.Button("Unlink"))
                {
                    node.SetParent(null);                    
                    //if I'm a logic node and I have no children, then set my parent mode to none
                    //if I'm a logic node and I have children, but the leaf node at the bottom is a logic node, set the whole chain to parent mode none
                    //if my parent was an orphaned logic node, set its parent mode to none

                    linkingNode.RemoveChild(node.entryAddress);
                    linkingNode = null;
                }
                break;
            case LinkMode.child:
                if (GUILayout.Button("Child"))
                {
                    linkingNode.AddChild(node.entryAddress);
                    node.SetParent(linkingNode);

                    //if my parent is a logic node with parent mode none, set its mode to mine
                    //if my parent is a logic node and I'm a player or NPC line, set its mode accordingly
                    if ((linkingNode as DialogueNode_Logic) != null)
                    {
                        if ((node as DialogueNode_Logic) != null)
                        {
                            if ((linkingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none)
                            {
                                (linkingNode as DialogueNode_Logic).SetParentEntryMode((node as DialogueNode_Logic).parentEntryMode);
                            }
                        }
                        if ((node as DialogueNode_Line) != null)
                        {
                            (linkingNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.NPCEntry);
                        }
                        if ((node as DialogueNode_Reply) != null)
                        {
                            (linkingNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.playerEntry);
                        }
                    }
                    //if I'm a logic node with parent mode none and my new parent is a logic node, set my parent mode to its
                    //if I'm a lodic node and my new parent is an NPC entry, set my parent mode to NPC
                    //if I'm a logic node and my new parent is a player entry, set my parent mode to player
                    
                    linkingNode = null;
                }
                break;
            case LinkMode.cancel:
                if (GUILayout.Button("Cancel"))
                {
                    linkingNode = null;
                }
                break;
        }
    }
    

    public void DrawCreateMenu()
    {
        bool proceed = false;
        GUILayout.BeginArea(new Rect(creationMenuPosition.x+scrollPosition.x,creationMenuPosition.y+scrollPosition.y,200,250), createMenuNodeStyle);
        {
            EditorGUILayout.LabelField("Create Node", EditorStyles.whiteLabel);

            GUILayout.BeginVertical();
            {
                //NPC Entry node creation
                if ((creatingNode == null) || (creatingNode as DialogueNode_Reply != null) || (creatingNode as DialogueNode_NPCEntry != null))
                {
                    proceed = false;

                    if (creatingNode as DialogueNode_Reply != null)
                    {
                        proceed = (creatingNode.GetChildren().Count == 0);
                    }

                    if (creatingNode as DialogueNode_NPCEntry != null)
                    {
                        proceed = true;
                        foreach(string s in creatingNode.GetChildren())
                        {
                            if ((currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry != null) ||
                                (currentDialogue.GetEntryByAddress(s) as DialogueNode_PlayerEntry != null))
                                proceed = false;
                        }
                        
                    }

                    if (creatingNode == null)
                        proceed = true;

                    if (proceed)
                    {
                        if (GUILayout.Button("NPC Tree"))
                        {
                            nodeTypeToCreate = typeof(DialogueNode_NPCEntry);
                            isCreatingNode = true;
                            showCreationMenu = false;
                        }
                    }
                    else
                        EditorGUILayout.LabelField(ObjectivesTabManager.StrikeThrough("NPC Tree"), EditorStyles.whiteLabel);
                }

                //Player Entry node creation
                if ((creatingNode == null) || (creatingNode as DialogueNode_NPCEntry!=null))
                {
                    proceed = false;                    

                    if (creatingNode as DialogueNode_NPCEntry != null)
                    {
                        proceed = true;
                        foreach (string s in creatingNode.GetChildren())
                        {
                            if ((currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry != null) ||
                                (currentDialogue.GetEntryByAddress(s) as DialogueNode_PlayerEntry != null))
                                proceed = false;
                        }
                    }

                    if (creatingNode == null)
                        proceed = true;

                    if (proceed)
                    {
                        if (GUILayout.Button("Player Tree"))
                        {
                            nodeTypeToCreate = typeof(DialogueNode_PlayerEntry);
                            isCreatingNode = true;
                            showCreationMenu = false;
                        }
                    }
                    else
                        EditorGUILayout.LabelField(ObjectivesTabManager.StrikeThrough("Player Tree"), EditorStyles.whiteLabel);
                }

                //NPC Line node creation
                if ((creatingNode == null) || (creatingNode as DialogueNode_NPCEntry!=null) || (creatingNode as DialogueNode_Logic!=null))
                {
                    proceed = false;
                    if (creatingNode as DialogueNode_Logic != null)
                    {
                        if (((creatingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.NPCEntry)||
                            ((creatingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none))
                        {
                            proceed = true;
                        }
                        else
                            proceed = false;
                    }
                    else
                        proceed = true;
                    if (proceed)
                    {
                        if (GUILayout.Button("NPC Line"))
                        {
                            nodeTypeToCreate = typeof(DialogueNode_Line);
                            isCreatingNode = true;
                            showCreationMenu = false;
                        }
                    }
                }

                //Reply Node creation
                if ((creatingNode == null) || (creatingNode as DialogueNode_PlayerEntry!=null) || (creatingNode as DialogueNode_Logic!=null))
                {
                    proceed = false;
                    if (creatingNode as DialogueNode_Logic != null)
                    {
                        if (((creatingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.playerEntry)||
                            ((creatingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none))
                        {
                            proceed = true;
                        }
                        else
                            proceed = false;
                    }
                    else
                        proceed = true;
                    if (proceed)
                    {
                        if (GUILayout.Button("Player Reply"))
                        {
                            nodeTypeToCreate = typeof(DialogueNode_Reply);
                            isCreatingNode = true;
                            showCreationMenu = false;
                        }
                    }
                }

                //Logic Node creation
                if ((creatingNode == null) || (creatingNode as DialogueNode_NPCEntry!=null) || (creatingNode as DialogueNode_Logic!=null) || (creatingNode as DialogueNode_PlayerEntry != null))
                {
                    if (GUILayout.Button("Logic - Token"))
                    {
                        nodeTypeToCreate = typeof(DialogueNode_LogicToken);
                        isCreatingNode = true;
                        showCreationMenu = false;
                    }
                }
                if ((creatingNode == null) || (creatingNode as DialogueNode_NPCEntry!=null) || (creatingNode as DialogueNode_Logic!=null) || (creatingNode as DialogueNode_PlayerEntry != null))
                {
                    if (GUILayout.Button("Logic - Inventory"))
                    {
                        nodeTypeToCreate = typeof(DialogueNode_LogicInventory);
                        isCreatingNode = true;
                        showCreationMenu = false;
                    }
                }
                if ((creatingNode == null) || (creatingNode as DialogueNode_NPCEntry!=null) || (creatingNode as DialogueNode_Logic!=null) || (creatingNode as DialogueNode_PlayerEntry != null))
                {
                    if (GUILayout.Button(ObjectivesTabManager.StrikeThrough("Logic - Quest (WIP)")))
                    {
                        nodeTypeToCreate = typeof(DialogueNode_LogicQuest);
                        isCreatingNode = true;
                        showCreationMenu = false;
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    isCreatingNode = false;
                    creatingNode = null;
                    showCreationMenu = false;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }
}
