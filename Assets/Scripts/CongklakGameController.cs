using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CongklakGameController : MonoBehaviour
{
    [Header("Game Board (assign in inspector)")]
    public Transform playerHolesParent;
    public Transform aiHolesParent;
    public Transform playerStore;
    public Transform aiStore;

    private Transform[] playerHoleTransforms = new Transform[HOLES_PER_SIDE];
    private Transform[] aiHoleTransforms = new Transform[HOLES_PER_SIDE];

    [Header("UI")]
    public GameObject gameEndPanel;
    public TextMeshProUGUI resultText;
    public Button backToMenuButton;
    public TextMeshProUGUI seedsInHandText; 
    public TextMeshProUGUI turnIndicatorText; 
    
    private const int HOLES_PER_SIDE = 7;
    private const int INITIAL_SEEDS = 7;
    private const int MAX_EXTRA_TURNS = 3;

    private int[] playerHoles = new int[HOLES_PER_SIDE];
    private int[] aiHoles = new int[HOLES_PER_SIDE];
    private int playerStoreCount = 0;
    private int aiStoreCount = 0;

    private bool isPlayerTurn = true;
    private bool isAnimating = false;
    private int consecutiveExtraTurns = 0;

    private List<Position> playerSowSeq;
    private List<Position> aiSowSeq;

    private float stepDelay = 0.4f; 
    private float moveDuration = 0.35f; 
    
    private Vector3 visualOffset = new Vector3(50f, -50f, 0f);

    enum PosKind { PlayerHole, PlayerStore, AIHole, AIStore }

    struct Position
    {
        public PosKind kind;
        public int index;
        public Position(PosKind k, int idx = -1) { kind = k; index = idx; }
    }

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        InitializeGame();
        backToMenuButton.onClick.AddListener(BackToMainMenu);
        if (gameEndPanel != null) gameEndPanel.SetActive(false);
        if (seedsInHandText != null) seedsInHandText.gameObject.SetActive(false); 
    }

    void InitializeGame()
    {
        for (int i = 0; i < HOLES_PER_SIDE; i++)
        {
            playerHoles[i] = INITIAL_SEEDS;
            aiHoles[i] = INITIAL_SEEDS;
        }
        playerStoreCount = 0;
        aiStoreCount = 0;
        isPlayerTurn = true;
        isAnimating = false;
        consecutiveExtraTurns = 0;

        BuildSowSequences();
        
        for (int i = 0; i < HOLES_PER_SIDE; i++)
        {
            if (playerHolesParent.childCount > i)
                playerHoleTransforms[i] = playerHolesParent.GetChild(i);
            
            if (aiHolesParent.childCount > i)
                aiHoleTransforms[i] = aiHolesParent.GetChild(i);
        }
        
        UpdateBoardVisuals();
    }

    void BuildSowSequences()
    {
        playerSowSeq = new List<Position>(HOLES_PER_SIDE * 2 + 1);
        aiSowSeq = new List<Position>(HOLES_PER_SIDE * 2 + 1);

        for (int i = 0; i < HOLES_PER_SIDE; i++) playerSowSeq.Add(new Position(PosKind.PlayerHole, i));
        playerSowSeq.Add(new Position(PosKind.PlayerStore));
        for (int i = 0; i < HOLES_PER_SIDE; i++) playerSowSeq.Add(new Position(PosKind.AIHole, i));

        for (int i = 0; i < HOLES_PER_SIDE; i++) aiSowSeq.Add(new Position(PosKind.AIHole, i));
        aiSowSeq.Add(new Position(PosKind.AIStore));
        for (int i = 0; i < HOLES_PER_SIDE; i++) aiSowSeq.Add(new Position(PosKind.PlayerHole, i));
    }

    public void OnPlayerHoleClicked(int holeIndex)
    {
        if (!isPlayerTurn || isAnimating) return;
        if (holeIndex < 0 || holeIndex >= HOLES_PER_SIDE) return;
        if (playerHoles[holeIndex] == 0) return;

        StartCoroutine(ExecuteMoveCoroutine(holeIndex, true));
    }

    private Transform GetTransformFromPosition(Position p)
    {
        if (p.kind == PosKind.PlayerHole) return playerHoleTransforms[p.index];
        if (p.kind == PosKind.AIHole) return aiHoleTransforms[p.index];
        if (p.kind == PosKind.PlayerStore) return playerStore;
        if (p.kind == PosKind.AIStore) return aiStore;
        return null;
    }

    IEnumerator ExecuteMoveCoroutine(int startHoleIndex, bool activeIsPlayer)
    {
        isAnimating = true;
        Debug.Log($"Start move by {(activeIsPlayer ? "Player" : "AI")} from hole {startHoleIndex}");

        List<Position> seq = activeIsPlayer ? playerSowSeq : aiSowSeq;
        int seedsInHand;

        if (activeIsPlayer)
        {
            seedsInHand = playerHoles[startHoleIndex];
            playerHoles[startHoleIndex] = 0;
        }
        else
        {
            seedsInHand = aiHoles[startHoleIndex];
            aiHoles[startHoleIndex] = 0;
        }
        
        UpdateBoardVisuals();
        
        Transform startHoleTransform = activeIsPlayer ? playerHoleTransforms[startHoleIndex] : aiHoleTransforms[startHoleIndex];

        if (seedsInHandText != null)
        {
            seedsInHandText.gameObject.SetActive(true);
            seedsInHandText.text = seedsInHand.ToString();
            
            if (startHoleTransform != null)
            {
                seedsInHandText.transform.position = startHoleTransform.position + visualOffset; 
            }
        }
        
        yield return new WaitForSeconds(stepDelay);

        int seqPos = -1;
        for (int i = 0; i < seq.Count; i++)
        {
            if ((activeIsPlayer && seq[i].kind == PosKind.PlayerHole && seq[i].index == startHoleIndex) ||
                (!activeIsPlayer && seq[i].kind == PosKind.AIHole && seq[i].index == startHoleIndex))
            {
                seqPos = i;
                break;
            }
        }
        if (seqPos == -1) seqPos = 0;

        int cursor = seqPos;
        bool extraTurn = false;

        while (true)
        {
            if (consecutiveExtraTurns >= MAX_EXTRA_TURNS)
            {
                Debug.Log($"MAX Extra Turns ({MAX_EXTRA_TURNS}) reached! Turn ends immediately.");
                break;
            }

            Position prevPos = seq[cursor]; 
            Transform startTransform = GetTransformFromPosition(prevPos);
            
            cursor = (cursor + 1) % seq.Count;
            Position p = seq[cursor];
            
            Transform targetTransform = GetTransformFromPosition(p);

            if (seedsInHandText != null && targetTransform != null)
            {
                Vector3 startPosition = seedsInHandText.transform.position;
                Vector3 targetPosition = targetTransform.position + visualOffset;
                
                float startTime = Time.time;
                
                while (Time.time < startTime + moveDuration)
                {
                    float t = (Time.time - startTime) / moveDuration;
                    seedsInHandText.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    yield return null;
                }
                seedsInHandText.transform.position = targetPosition;
            }

            if (p.kind == PosKind.PlayerHole)
            {
                playerHoles[p.index]++;
                seedsInHand--;
                
                if (seedsInHandText != null) seedsInHandText.text = seedsInHand.ToString();
                
                UpdateBoardVisuals();
                yield return new WaitForSeconds(stepDelay - moveDuration); 

                if (seedsInHand == 0)
                {
                    int now = playerHoles[p.index];
                    
                    if (now > 1)
                    {
                        if (consecutiveExtraTurns >= MAX_EXTRA_TURNS)
                        {
                            Debug.Log($"MAX Extra Turns ({MAX_EXTRA_TURNS}) reached! Cannot continue from hole.");
                            break;
                        }
                        
                        seedsInHand = now;
                        playerHoles[p.index] = 0;
                        
                        if (seedsInHandText != null) 
                        {
                            seedsInHandText.text = seedsInHand.ToString();
                            seedsInHandText.transform.position = GetTransformFromPosition(p).position + visualOffset; 
                        }
                        
                        consecutiveExtraTurns++;
                        Debug.Log($"{(activeIsPlayer ? "Player" : "AI")} **Lanjut Jalan** dari lubang Player {p.index}. Streak: {consecutiveExtraTurns}");
                        UpdateBoardVisuals();
                        yield return new WaitForSeconds(stepDelay);
                        continue;
                    }
                    else
                    {
                        Debug.Log($"Landed on empty hole {p.index}. Turn ends.");
                        break; 
                    }
                }
            }
            else if (p.kind == PosKind.AIHole)
            {
                aiHoles[p.index]++;
                seedsInHand--;
                
                if (seedsInHandText != null) seedsInHandText.text = seedsInHand.ToString();
                
                UpdateBoardVisuals();
                yield return new WaitForSeconds(stepDelay - moveDuration); 

                if (seedsInHand == 0)
                {
                    int now = aiHoles[p.index];
                    
                    if (now > 1)
                    {
                        if (consecutiveExtraTurns >= MAX_EXTRA_TURNS)
                        {
                            Debug.Log($"MAX Extra Turns ({MAX_EXTRA_TURNS}) reached! Cannot continue from hole.");
                            break;
                        }
                        
                        seedsInHand = now;
                        aiHoles[p.index] = 0;
                        
                        if (seedsInHandText != null) 
                        {
                            seedsInHandText.text = seedsInHand.ToString();
                            seedsInHandText.transform.position = GetTransformFromPosition(p).position + visualOffset; 
                        }
                        
                        consecutiveExtraTurns++;
                        Debug.Log($"{(activeIsPlayer ? "Player" : "AI")} **Lanjut Jalan** dari lubang AI {p.index}. Streak: {consecutiveExtraTurns}");
                        UpdateBoardVisuals();
                        yield return new WaitForSeconds(stepDelay);
                        continue;
                    }
                    else
                    {
                        Debug.Log($"Landed on empty hole {p.index}. Turn ends.");
                        break;
                    }
                }
            }
            else if (p.kind == PosKind.PlayerStore)
            {
                if (activeIsPlayer)
                {
                    playerStoreCount++;
                    seedsInHand--;
                    
                    if (seedsInHandText != null) seedsInHandText.text = seedsInHand.ToString();
                    
                    UpdateBoardVisuals();
                    yield return new WaitForSeconds(stepDelay - moveDuration); 

                    if (seedsInHand == 0)
                    {
                        if (consecutiveExtraTurns < MAX_EXTRA_TURNS)
                        {
                            extraTurn = true;
                            consecutiveExtraTurns++;
                            Debug.Log($"Player landed in store. Extra Turn granted. Streak: {consecutiveExtraTurns}");
                        }
                        else
                        {
                            extraTurn = false;
                            Debug.Log($"MAX Extra Turns ({MAX_EXTRA_TURNS}) reached! Turn ends.");
                        }
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if (p.kind == PosKind.AIStore)
            {
                if (!activeIsPlayer)
                {
                    aiStoreCount++;
                    seedsInHand--;
                    
                    if (seedsInHandText != null) seedsInHandText.text = seedsInHand.ToString();
                    
                    UpdateBoardVisuals();
                    yield return new WaitForSeconds(stepDelay - moveDuration); 

                    if (seedsInHand == 0)
                    {
                        if (consecutiveExtraTurns < MAX_EXTRA_TURNS)
                        {
                            extraTurn = true;
                            consecutiveExtraTurns++;
                            Debug.Log($"AI landed in store. Extra Turn granted. Streak: {consecutiveExtraTurns}");
                        }
                        else
                        {
                            extraTurn = false;
                            Debug.Log($"MAX Extra Turns ({MAX_EXTRA_TURNS}) reached! Turn ends.");
                        }
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        isAnimating = false;
        
        if (seedsInHandText != null)
        {
            Vector3 startPos = seedsInHandText.transform.position;
            Vector3 endPos = startPos + Vector3.down * 200f; 
            
            float startTime = Time.time;
            float fadeDuration = 0.5f;
            
            while (Time.time < startTime + fadeDuration)
            {
                float t = (Time.time - startTime) / fadeDuration;
                seedsInHandText.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            seedsInHandText.gameObject.SetActive(false); 
        }
        
        Debug.Log($"End move by {(activeIsPlayer ? "Player" : "AI")}. extraTurn={extraTurn}");

        if (CheckAndHandleGameEnd()) yield break;

        if (!extraTurn)
        {
            isPlayerTurn = !isPlayerTurn;
            consecutiveExtraTurns = 0;
        }

        if (!isPlayerTurn)
        {
            if (CanActivePlayerMove())
            {
                yield return new WaitForSeconds(stepDelay * 2); 
                MakeAIMove();
            }
            else
            {
                Debug.Log("AI cannot move. Passing turn back to Player.");
                isPlayerTurn = true;
                consecutiveExtraTurns = 0;
                UpdateBoardVisuals(); 
            }
        }
        
        UpdateBoardVisuals();
    }
    
    private bool CanActivePlayerMove()
    {
        int[] holes = isPlayerTurn ? playerHoles : aiHoles;
        for (int i = 0; i < HOLES_PER_SIDE; i++)
        {
            if (holes[i] > 0) return true;
        }
        return false;
    }

    void MakeAIMove()
    {
        if (isAnimating) return;

        for (int i = 0; i < HOLES_PER_SIDE; i++)
        {
            if (aiHoles[i] > 0)
            {
                StartCoroutine(ExecuteMoveCoroutine(i, false));
                break;
            }
        }
    }
    
    private bool CheckAndHandleGameEnd()
    {
        bool playerHasSeeds = false;
        bool aiHasSeeds = false;
        for (int i = 0; i < HOLES_PER_SIDE; i++)
        {
            if (playerHoles[i] > 0) playerHasSeeds = true;
            if (aiHoles[i] > 0) aiHasSeeds = true;
        }

        if (!playerHasSeeds || !aiHasSeeds)
        {
            for (int i = 0; i < HOLES_PER_SIDE; i++)
            {
                playerStoreCount += playerHoles[i];
                aiStoreCount += aiHoles[i];
                playerHoles[i] = 0;
                aiHoles[i] = 0;
            }
            UpdateBoardVisuals();
            EndGame();
            return true;
        }
        return false;
    }

    void EndGame()
    {
        if (gameEndPanel != null) gameEndPanel.SetActive(true);
        if (resultText == null) return;

        if (playerStoreCount > aiStoreCount)
            resultText.text = $"YOU WIN!\n{playerStoreCount} - {aiStoreCount}";
        else if (playerStoreCount < aiStoreCount)
            resultText.text = $"YOU LOSE!\n{playerStoreCount} - {aiStoreCount}";
        else
            resultText.text = $"DRAW!\n{playerStoreCount} - {aiStoreCount}";
    }

    void UpdateBoardVisuals()
    {
        if (playerHolesParent != null)
        {
            for (int i = 0; i < HOLES_PER_SIDE; i++)
            {
                Transform hole = playerHolesParent.GetChild(i);
                if (hole != null)
                {
                    TextMeshProUGUI t = hole.GetComponentInChildren<TextMeshProUGUI>();
                    if (t) t.text = playerHoles[i].ToString();
                    Button b = hole.GetComponent<Button>();
                    if (b != null) b.interactable = isPlayerTurn && !isAnimating && playerHoles[i] > 0;
                }
            }
        }

        if (aiHolesParent != null)
        {
            for (int i = 0; i < HOLES_PER_SIDE; i++)
            {
                Transform hole = aiHolesParent.GetChild(i);
                if (hole != null)
                {
                    TextMeshProUGUI t = hole.GetComponentInChildren<TextMeshProUGUI>();
                    if (t) t.text = aiHoles[i].ToString();
                }
            }
        }

        if (playerStore != null)
        {
            TextMeshProUGUI t = playerStore.GetComponentInChildren<TextMeshProUGUI>();
            if (t) t.text = playerStoreCount.ToString();
        }
        if (aiStore != null)
        {
            TextMeshProUGUI t = aiStore.GetComponentInChildren<TextMeshProUGUI>();
            if (t) t.text = aiStoreCount.ToString();
        }
        
        if (turnIndicatorText != null)
        {
            if (isPlayerTurn)
            {
                turnIndicatorText.text = "YOUR TURN";
                turnIndicatorText.color = Color.green;
            }
            else
            {
                turnIndicatorText.text = "OPPONENT'S TURN";
                turnIndicatorText.color = Color.red;
            }
        }
    }

    void BackToMainMenu()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("MainMenu");
    }
}