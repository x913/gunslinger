#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;

namespace GunslingerGame
{
	/// <summary>
	/// Includes functions for loading levels and URLs. It's intended for use with UI Buttons
	/// </summary>
	public class GSGLoadLevel  : MonoBehaviour, IUnityAdsListener, IStoreListener
	{
		public GameObject NoAdsButton;

		// ADV

		string gameId = "3737950";
		string myPlacementId = "inter";
		bool testMode = false;

		// Purchases
		private IStoreController controller;
		private IExtensionProvider extensions;

		private string noAdsPurchase = "adv_free_ios";

		// How many seconds to wait before loading a level or URL
		public float loadDelay = 1;

		// The name of the URL to be loaded
		public string urlName = "";

		// The name of the level to be loaded
		public string levelName = "";

		// load sounds and its source
		public AudioClip soundLoad;
		public string soundSourceTag = "GameController";
		public GameObject soundSource;

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			// initialize ads
			Advertisement.AddListener(this);
			Advertisement.Initialize(gameId, testMode);

			// initialize IAP
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.AddProduct(noAdsPurchase, ProductType.NonConsumable, new IDs { { noAdsPurchase, MacAppStore.Name } });
			UnityPurchasing.Initialize(this, builder);

			// If there is no sound source assigned, try to assign it from the tag name
			if ( !soundSource && GameObject.FindGameObjectWithTag(soundSourceTag) )    
				soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);
		}

		public void ShowRewardedVideo()
		{
			// Check if UnityAds ready before calling Show method:
			if (Advertisement.IsReady(myPlacementId))
			{
				Advertisement.Show(myPlacementId);
			}
			else
			{
				Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
				RealLoadLevel();
			}
		}


		/// <summary>
		/// Loads the URL.
		/// </summary>
		/// <param name="urlName">URL/URI</param>
		public void LoadURL()
		{
			Time.timeScale = 1;

			// If there is a sound, play it from the source
			if ( soundSource && soundLoad )    soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);

			// Execute the function after a delay
			Invoke("ExecuteLoadURL", loadDelay);
		}

		/// <summary>
		/// Executes the load URL function
		/// </summary>
		void ExecuteLoadURL()
		{
			Application.OpenURL(urlName);
		}
	
		/// <summary>
		/// Loads the level.
		/// </summary>
		/// <param name="levelName">Level name.</param>
		public void LoadLevel()
		{
			if (!hasReceipt(noAdsPurchase))
			{
				ShowRewardedVideo();
			} else
			{
				RealLoadLevel();
			}
		}

		public void RealLoadLevel()
		{
			Time.timeScale = 1;
			// If there is a sound, play it from the source
			if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);
			// Execute the function after a delay
			Invoke("ExecuteLoadLevel", loadDelay);
		}

		/// <summary>
		/// Executes the Load Level function
		/// </summary>
		void ExecuteLoadLevel()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(levelName);
			#else
			Application.LoadLevel(levelName);
			#endif
		}

		/// <summary>
		/// Restarts the current level.
		/// </summary>
		public void RestartLevel()
		{
			Time.timeScale = 1;
			// If there is a sound, play it from the source
			if (soundSource && soundLoad) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLoad);
			// Execute the function after a delay
			Invoke("ExecuteRestartLevel", loadDelay);
		}
		
		/// <summary>
		/// Executes the Load Level function
		/// </summary>
		void ExecuteRestartLevel()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			#else
			Application.LoadLevel(Application.loadedLevelName);
			#endif
		}

		public void OnUnityAdsReady(string placementId)
		{
			//throw new System.NotImplementedException();
		}

		public void OnUnityAdsDidError(string message)
		{ 
			RealLoadLevel();
		}

		public void OnUnityAdsDidStart(string placementId)
		{
			//throw new System.NotImplementedException();
		}

		public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
		{
			RealLoadLevel();
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			this.controller = controller;
			this.extensions = extensions;

			if(!hasReceipt(noAdsPurchase))
			{
				Debug.Log("NO RECEIPT FOR " + noAdsPurchase);
				if(NoAdsButton != null)
					NoAdsButton.SetActive(true);
			}

		}

		public bool hasReceipt(string productId)
		{
			if (controller == null)
				return false;

			Product product = controller.products.WithID(productId);
			return product != null && product.hasReceipt;
		}

		public void InitiateNoAdsPurchase()
		{
			if (controller != null)
				controller.InitiatePurchase(noAdsPurchase);
			else
				Debug.Log("controller is null");
		}

	}
}