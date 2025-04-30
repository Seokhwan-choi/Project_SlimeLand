using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace MLand
{
    class IAPManager : IStoreListener
    {
        static int mInitCount;
        static IStoreController mStoreController;
        static IExtensionProvider mStoreExtenstionProvider;
        static Action mSuccessPurchased;
        static Action mFailedPurchased;
        //상품의 정보와 상품의 이름을 넣은 빌더를 넣어 초기화
        public void Init()
        {
            //if (mStoreController != null && mStoreExtenstionProvider != null) return;

            //if (mInitCount > 3) return;

            //ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //foreach (GemShopData data in MLand.GameData.GemShopData.Values)
            //{
            //    if (data.productId.IsValid())
            //    {
            //        ProductType type = data.onlyOne ? ProductType.NonConsumable : ProductType.Consumable;

            //        builder.AddProduct(data.productId, type, new IDs { { data.id, GooglePlay.Name } });
            //    }
            //}

            //UnityPurchasing.Initialize(this, builder);

            mInitCount++;
        }

        // 구매 시작
        public void StartPurchase(string productId, Action onSuccessPurchased, Action onFailedPurchased)
        {
            mSuccessPurchased = onSuccessPurchased;
            mFailedPurchased = onFailedPurchased;

            // 구매 프로세스 시작
            mStoreController?.InitiatePurchase(productId);
        }

        public bool IsAvailableToPurchase(string productId)
        {
            if (mStoreController != null)
            {
                Product product = mStoreController.products.WithID(productId);

                return product?.availableToPurchase ?? false;
            }

            return false;
        }

        public string GetProductPrice(string productId)
        {
            if (mStoreController != null && productId.IsValid())
            {
                Product product = mStoreController.products.WithID(productId);

                return product?.metadata.localizedPriceString ?? "---";
            }

            return "---";
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // 초기화 성공
            mStoreController = controller;
            mStoreExtenstionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"{error}");

            MLand.GameManager.StartCoroutine(DelayedTryInit());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"{error} : {message}");

            MLand.GameManager.StartCoroutine(DelayedTryInit());
        }

        // 결제 시도
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            bool validPurchase = true;

            //클라이언트 영수증 검산
            //영수증 확인을 통해 사용자가 구매하지 않은 콘텐츠에 액세스하지 못하게 합니다.
            //https://docs.unity3d.com/kr/current/Manual/UnityIAPValidatingReceipts.html
#if UNITY_ANDROID && !UNITY_EDITOR
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                //영수증 검사
                /*서명 검증을 통해 영수증 유효성을 검사합니다.
                영수증의 애플리케이션 번들 식별자를 애플리케이션의 식별자와 비교합니다. 
                이 둘이 일치하지 않으면 InvalidBundleId 예외 오류가 발생합니다.*/
                var result = validator.Validate(e.purchasedProduct.receipt);

                //영수증 내용 출력
                foreach (IPurchaseReceipt purchaseReceipt in result)
                {
                    Debug.Log(purchaseReceipt.productID);
                    Debug.Log(purchaseReceipt.purchaseDate);
                    Debug.Log(purchaseReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                mFailedPurchased?.Invoke();

                validPurchase = false;
            }
#endif
            if (validPurchase)
            {
                mSuccessPurchased?.Invoke();
            }

            return PurchaseProcessingResult.Complete;
        }

        // 결제 실패
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (product.Equals(PurchaseFailureReason.UserCancelled) == false)
                mFailedPurchased?.Invoke();
        }

        IEnumerator DelayedTryInit()
        {
            yield return new WaitForSeconds(10f);

            Init();
        }
    }
}
