using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterGaugeInteraction : MonoBehaviour
{
    public Image gaugeImage; // Gauge_Frame의 Image 컴포넌트
    public float fixedIncreaseAmount = 0.1f; // 터치 시 게이지 증가량
    public float fillDecreaseSpeed = 0.1f; // 게이지 감소 속도
    public float delayBeforeDecrease = 1f; // 게이지 감소 전 대기 시간
    public GameObject resultSuccess; // Result_Success 오브젝트
    public GameObject resultFailed; // Result_Failed 오브젝트

    private bool isGaugeDecreasing = true; // 게이지 감소 상태 플래그
    private bool isCaptureFailed = false; // 포획 실패 상태 플래그
    private bool resultFailedActive = false; // Result_Failed 활성화 상태 플래그
    private bool resultSuccessActive = false; // Result_Success 활성화 상태 플래그

    public RectTransform frameCenter; // Frame_Center의 RectTransform
    private Rect frameRect;

    void Start()
    {
        // Frame_Center의 RectTransform을 스크린 좌표로 변환
        Vector3[] frameCorners = new Vector3[4];
        frameCenter.GetWorldCorners(frameCorners);
        frameRect = new Rect(frameCorners[0].x, frameCorners[0].y,
                             frameCorners[2].x - frameCorners[0].x,
                             frameCorners[2].y - frameCorners[0].y);

        // 게이지 감소를 딜레이 후에 시작
        StartCoroutine(StartDecreasingGaugeWithDelay());
    }

    void Update()
    {
        // 터치 입력 감지 (포획 실패 상태가 아닐 때만)
        if (Input.GetMouseButtonDown(0) && !isCaptureFailed && !resultSuccessActive)
        {
            Vector3 touchPosition = Input.mousePosition;

            // 터치 위치가 Frame_Center 안에 있는지 확인
            if (frameRect.Contains(touchPosition))
            {
                IncreaseGaugeFixedAmount();
            }
        }
    }

    // 게이지 감소 시작 (딜레이 포함)
    System.Collections.IEnumerator StartDecreasingGaugeWithDelay()
    {
        Debug.Log("게이지 감소를 위한 대기 시간 시작.");
        // 지정된 딜레이 시간 동안 대기
        yield return new WaitForSeconds(delayBeforeDecrease);

        Debug.Log("게이지 감소 시작!");
        // 매 프레임마다 게이지 감소
        StartCoroutine(DecreaseGauge());
    }

    // 게이지를 점진적으로 감소시키는 코루틴
    System.Collections.IEnumerator DecreaseGauge()
    {
        while (gaugeImage.fillAmount > 0f && isGaugeDecreasing)
        {
            gaugeImage.fillAmount -= fillDecreaseSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 게이지가 모두 소모되었을 때
        if (gaugeImage.fillAmount <= 0f)
        {
            Debug.Log("게이지가 모두 소모되었습니다.");

            // 포획 실패 상태로 설정
            isCaptureFailed = true;

            // Result_Failed 활성화 및 상태 플래그 설정
            resultFailed.SetActive(true);
            resultFailedActive = true;

            // 5초 후에 메인 화면으로 이동
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("Main Scene");
        }
    }

    // 터치 시 고정된 양만큼 게이지 증가
    void IncreaseGaugeFixedAmount()
    {
        // 게이지 증가
        gaugeImage.fillAmount += fixedIncreaseAmount;

        // 게이지가 최대치에 도달하면 성공 처리
        if (gaugeImage.fillAmount >= 1f)
        {
            gaugeImage.fillAmount = 1f; // 게이지 최대치 고정
            Debug.Log("게이지 최대치에 도달!");

            // 게이지 감소 중지
            isGaugeDecreasing = false;

            // Result_Success 활성화 및 상태 플래그 설정
            resultSuccess.SetActive(true);
            resultSuccessActive = true;

            // 5초 후에 메인 화면으로 이동
            StartCoroutine(GoToMainSceneAfterDelay(5f));
        }
    }

    // 5초 후 메인 화면으로 이동
    System.Collections.IEnumerator GoToMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main Scene");
    }

    // 터치로 Result_Failed 또는 Result_Success 창이 비활성화되는 것을 방지
    void LateUpdate()
    {
        if (resultFailedActive && !resultFailed.activeSelf)
        {
            resultFailed.SetActive(true);
        }

        if (resultSuccessActive && !resultSuccess.activeSelf)
        {
            resultSuccess.SetActive(true);
        }
    }
}
