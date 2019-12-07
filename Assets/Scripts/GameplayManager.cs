using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class GameplayManager : MonoBehaviour {

    // public variables

    public const float dotRadius = 0.3f;

    public int highScore = 0;

    public Player player;
    public GameObject dot;
    public Text scoreText;
    public Canvas canvas;
    private Animator canvasAnimator;
    public GameObject mainCamera;

    public float minGravity = -20;
    public float maxGravity = -80;

    public float minArcHeight = 1.6f;
    public float maxArcHeight = 5.0f;

    public float minArcWidth = 1.0f;
    public float maxArcWidth = 2.0f;

    public int scorePerDotHit = 1;
    public float gravityDecrementPerHit = 0.5f;

    // private variables
    private System.Random random = new System.Random();

    private Collider2D playerCollider;
    private Collider2D dotCollider;

    private float timeToSpawnNewDot;
    private int score;

    private Vector3 cameraDimensions;

    enum AppState {Menu, Playing, Lost, GameOver};

    AppState appState = AppState.Menu;

    // Start is called before the first frame update
    void Awake() {
        highScore = PlayerPrefs.GetInt("High_Score");

        UpdateGravity(minGravity);

        playerCollider = player.GetComponent<Collider2D>();
        dotCollider = dot.GetComponent<Collider2D>();

        score = 0;
        timeToSpawnNewDot = 0.6f;
        appState = AppState.Menu;

        cameraDimensions = GetCameraDimensions();
        canvasAnimator = canvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && appState == AppState.Menu) {
            appState = AppState.Playing;
            Jump();
            SpawnDot();
            canvasAnimator.SetTrigger("Game Start");
            canvasAnimator.ResetTrigger("Restart");
        } else if (Input.GetMouseButtonDown(0) && appState == AppState.Playing) {
            if (!playerCollider.bounds.Intersects(dotCollider.bounds)) {
                appState = AppState.Lost;
                return;
            }

            FindObjectOfType<AudioManager>().Play("Pop", (float)random.NextDouble()*(1.1f-0.9f) + 0.9f);
            player.SetDirection((player.transform.position.x <= 0) ? 1 : -1);
            Jump();
            RandomizeDotSpawnTime();
            SpawnDot();

            AddScore(scorePerDotHit);
            UpdateGravity(Math.Max(maxGravity, Physics2D.gravity.y - gravityDecrementPerHit));
            player.UpdateRotation();
        } else if (Input.GetMouseButtonDown(0) && appState == AppState.GameOver) {
            Restart();
        }

        Vector3 CamDimensions = GetCameraDimensions();
        if (Math.Abs(player.transform.position.x) > CamDimensions.x
            || player.transform.position.y < Camera.main.transform.position.y - 5.0f
            && (appState == AppState.Playing
            || appState == AppState.Lost)) {
            GameOver();
        }

    }

    private void Restart() {
        appState = AppState.Menu;
        canvasAnimator.SetTrigger("Restart");
        canvasAnimator.ResetTrigger("Game Over");
     
        player.transform.position = new Vector3(0, 0.9f, 0);
        player.transform.rotation = new Quaternion();
        mainCamera.transform.position = new Vector3(0, 5.0f, -10.0f);
        UpdateGravity(minGravity);

        cameraDimensions = GetCameraDimensions();

        player.SetArcHeight(0.0f);
        player.SetArcWidth(0.0f);
        player.rigidBody2D.velocity = new Vector2(0.0f, 0.0f);
        timeToSpawnNewDot = 0.6f;
        score = 0;
    }

    private void GameOver() {
        appState = AppState.GameOver;
        if (score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt("High_Score", highScore);
            PlayerPrefs.Save();
        }
        UpdateScore();

        canvasAnimator.SetTrigger("Game Over");
        canvasAnimator.ResetTrigger("Game Start");
    }

    private void Jump() {
        player.SetArcHeight((float)random.NextDouble() * (maxArcHeight - minArcHeight) + minArcHeight);
        player.SetArcWidth((float)random.NextDouble() * (maxArcWidth- minArcWidth) + minArcWidth);
        player.UpdateVelocity();
    }

    private void UpdateGravity(float y) {
        Physics2D.gravity = new Vector2(0.0f, y);
    }

    private void RandomizeDotSpawnTime() {
        float changeInX = cameraDimensions.x + Math.Abs(player.transform.position.x) - dotRadius;
        float changeInY = (Camera.main.transform.position.y-5.0f) - player.transform.position.y + 2 * dotRadius;

        float minTime = -player.GetVelocityY() / Physics2D.gravity.y;

        float highestHeight = (player.GetVelocityY()*minTime) + (0.5f * Physics2D.gravity.y * (float)Math.Pow(minTime, 2)) + player.transform.position.y;
        
        if (highestHeight > Camera.main.transform.position.y) {
            changeInY += (highestHeight-Camera.main.transform.position.y);
        }

        float maxXTime = (changeInX) / player.GetVelocityX();
        float maxYTime = Quadratic(0.5f*Physics2D.gravity.y, player.GetVelocityY(), (-changeInY));

        timeToSpawnNewDot = (float)random.NextDouble() * (Math.Min(maxXTime, maxYTime)-minTime) + minTime;
    }

    private float Quadratic(float a, float b, float c) {
        return (-b - (float)Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
    }

    private void SpawnDot() {
        float relNewX = player.GetVelocityX() * player.GetDirection() * timeToSpawnNewDot;
        float relNewY = (player.GetVelocityY() * timeToSpawnNewDot) + (0.5f * Physics2D.gravity.y * (float)Math.Pow((double)timeToSpawnNewDot, 2));

        float newX = (float)(player.transform.position.x + relNewX);
        float newY = (float)(player.transform.position.y + relNewY - dotRadius);

        dot.transform.position = new Vector3(newX, newY, 0);
    }

    private void AddScore(int x) {
        score += x;
    }

    private void UpdateScore() {
        scoreText.text = "Score: " + score + " Best: " + highScore;
    }

    private Vector3 GetCameraDimensions() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }
}
