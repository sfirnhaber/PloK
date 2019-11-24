using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class GameplayManager : MonoBehaviour {

    // public variables

    public const float dotRadius = 0.3f;

    public Player player;
    public GameObject dot;
    public Text score;

    public int dotsHit;

    public float minGravity = -20;
    public float maxGravity = -80;

    public float minArcHeight = 1.6f;
    public float maxArcHeight = 5.0f;

    public float minArcWidth = 1.0f;
    public float maxArcWidth = 2.0f;

    // private variables
    private System.Random random = new System.Random();

    private Collider2D playerCollider;
    private Collider2D dotCollider;

    private float timeToSpawnNewDot;
    private bool running;

    private Vector3 cameraDimensions;

    // Start is called before the first frame update
    void Awake() {
        UpdateGravity(minGravity);

        playerCollider = player.GetComponent<Collider2D>();
        dotCollider = dot.GetComponent<Collider2D>();

        dotsHit = 0;
        timeToSpawnNewDot = 0.6f;
        running = true;

        Jump();
        SpawnDot();
        cameraDimensions = GetCameraDimensions();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonDown(0) && running) {
            if (!playerCollider.bounds.Intersects(dotCollider.bounds)) {
                running = false;
                return;
            }

            player.SetDirection((player.transform.position.x <= 0) ? 1 : -1);
            Jump();
            RandomizeDotSpawnTime();
            SpawnDot();

            AddScore(1);
            UpdateGravity(Math.Max(maxGravity, Physics2D.gravity.y - 2));
            player.UpdateRotation();
        }
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
        dotsHit += x;
        score.text = "Score: " + (dotsHit * 10).ToString();
    }

    private Vector3 GetCameraDimensions() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }
}
