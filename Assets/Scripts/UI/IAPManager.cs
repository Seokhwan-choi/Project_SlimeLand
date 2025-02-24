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
        //��ǰ�� ������ ��ǰ�� �̸��� ���� ������ �־� �ʱ�ȭ
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

        // ���� ����
        public void StartPurchase(string productId, Action onSuccessPurchased, Action onFailedPurchased)
        {
            mSuccessPurchased = onSuccessPurchased;
            mFailedPurchased = onFailedPurchased;

            // ���� ���μ��� ����
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
            // �ʱ�ȭ ����
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

        // ���� �õ�
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            bool validPurchase = true;

            //Ŭ���̾�Ʈ ������ �˻�
            //������ Ȯ���� ���� ����ڰ� �������� ���� �������� �׼������� ���ϰ� �մϴ�.
            //https://docs.unity3d.com/kr/current/Manual/UnityIAPValidatingReceipts.html
#if UNITY_ANDROID && !UNITY_EDITOR
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                //������ �˻�
                /*���� ������ ���� ������ ��ȿ���� �˻��մϴ�.
                �������� ���ø����̼� ���� �ĺ��ڸ� ���ø����̼��� �ĺ��ڿ� ���մϴ�. 
                �� ���� ��ġ���� ������ InvalidBundleId ���� ������ �߻��մϴ�.*/
                var result = validator.Validate(e.purchasedProduct.receipt);

                //������ ���� ���
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

        // ���� ����
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
