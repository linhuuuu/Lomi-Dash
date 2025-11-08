// using UnityEngine;
// using System;
// using Firebase.Firestore;
// using System.Threading.Tasks;
// using Firebase.Auth;
// using System.Collections.Generic;

// public class PlaytimeTracker : MonoBehaviour
// {
//     public static PlaytimeTracker Instance { get; private set; }

//     [SerializeField] private float TotalSeconds = 0f;

//     private DateTime sessionStartRealTime;
//     private float sessionStartTime; // Time.time at start

//     private bool isLoaded = false;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Initialize();
//     }

//     private async void Initialize()
//     {
//         sessionStartRealTime = DateTime.Now;
//         sessionStartTime = Time.time;

//         // Try to load from Firebase first
//         if (Application.internetReachability != NetworkReachability.NotReachable)
//         {
//             await LoadFromFirebase();
//         }
//         else
//         {
//             LoadFromPlayerPrefs(); // Offline fallback
//         }

//         isLoaded = true;
//         Debug.Log($"[Playtime] Started session. Total so far: {FormatTime(TotalSeconds)}");
//     }

//     private async void OnApplicationPause(bool pauseStatus)
//     {
//         if (pauseStatus && isLoaded)
//             await SaveProgress();
//     }

//     private async void OnApplicationQuit()
//     {
//         if (isLoaded)
//             await SaveProgress();
//     }

//     private void Update()
//     {
//         if (!isLoaded) return;
//         TotalSeconds = GetSessionDuration() + PlayerPrefs.GetFloat("TotalPlaytime_Local", 0f);
//     }

//     // Duration of current session only
//     private float GetSessionDuration()
//     {
//         return Time.time - sessionStartTime;
//     }

//     // --- SAVE LOGIC ---

//     private async Task SaveProgress()
//     {
//         float sessionDuration = GetSessionDuration();

//         // Update local total
//         float newLocalTotal = PlayerPrefs.GetFloat("TotalPlaytime_Local", 0f) + sessionDuration;
//         PlayerPrefs.SetFloat("TotalPlaytime_Local", newLocalTotal);
//         PlayerPrefs.SetString("LastPlayed", DateTime.Now.ToString("O")); // ISO 8601
//         PlayerPrefs.Save();

//         // Upload to Firebase
//         await SaveToCloud(newLocalTotal);
//     }

//     // --- CLOUD: LOAD & SAVE ---

//     private async Task LoadFromFirebase()
//     {
//         if (FirebaseAuth.DefaultInstance.CurrentUser == null)
//         {
//             LoadFromPlayerPrefs();
//             return;
//         }

//         string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
//         DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("players").Document(uid);

//         try
//         {
//             DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

//             if (snapshot.Exists)
//             {
//                 var data = snapshot.ToDictionary();

//                 // Use cloud total if exists
//                 if (data.TryGetValue("stats.totalPlaytime", out object cloudVal) && cloudVal is double cloudSec)
//                 {
//                     TotalSeconds = (float)cloudSec;
//                     PlayerPrefs.SetFloat("TotalPlaytime_Local", TotalSeconds); // Sync local
//                     PlayerPrefs.Save();
//                     Debug.Log($"[Playtime] Loaded from cloud: {FormatTime(TotalSeconds)}");
//                     return;
//                 }
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogWarning("[Playtime] Failed to load from cloud: " + e.Message);
//         }

//         // Fallback to local
//         LoadFromPlayerPrefs();
//     }

//     private async Task SaveToCloud(float totalSeconds)
//     {
//         if (FirebaseAuth.DefaultInstance.CurrentUser == null) return;

//         string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
//         DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("players").Document(uid);

//         var updates = new Dictionary<FieldPath, object>
//         {
//             { new FieldPath("stats", "totalPlaytime"), totalSeconds },
//             { new FieldPath("analytics", "lastPlayedAt"), Timestamp.GetCurrentTimestamp() },
//             { new FieldPath("analytics", "sessionCount"), FieldValue.Increment(1) }
//         };

//         try
//         {
//             await docRef.SetAsync(updates, SetOptions.MergeAll);
//             Debug.Log($"[Playtime] Saved to cloud: {FormatTime(totalSeconds)}");
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("[Playtime] Cloud save failed: " + e.Message);
//             // Optionally queue for retry
//         }
//     }

//     // --- LOCAL FALLBACK ---

//     private void LoadFromPlayerPrefs()
//     {
//         TotalSeconds = PlayerPrefs.GetFloat("TotalPlaytime_Local", 0f);
//         Debug.Log($"[Playtime] Loaded from local: {FormatTime(TotalSeconds)}");
//     }

//     // --- FORMATTING ---

//     public string FormatTime(float totalSeconds)
//     {
//         TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
//         if (t.TotalHours >= 1)
//             return $"{t.Hours}h {t.Minutes}m";
//         else if (t.TotalMinutes >= 1)
//             return $"{t.Minutes}m {t.Seconds}s";
//         else
//             return $"{t.Seconds}s";
//     }

//     // Optional: Expose session start
//     public DateTime SessionStartRealTime => sessionStartRealTime;
// }