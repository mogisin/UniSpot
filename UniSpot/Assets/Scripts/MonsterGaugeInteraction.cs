using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterGaugeInteraction : MonoBehaviour
{
    public Image gaugeImage; // Gauge_Frame의 Image 컴포넌트
    public float fillSpeed = 0.5f; // 게이지 증가 속도
    public float delayBeforeIncrease = 1f; // 게이지 증가 전 대기 시간
    public GameObject resultSuccess; // Result_Success 오브젝트

    void Start()
    {
        // 게이지 증가를 딜레이 후에 시작
        StartCoroutine(StartIncreasingGaugeWithDelay());
    }

    // 게이지 증가 시작 (딜레이 포함)
    System.Collections.IEnumerator StartIncreasingGaugeWithDelay()
    {
        Debug.Log("게이지 증가를 위한 대기 시간 시작.");
        // 지정된 딜레이 시간 동안 대기
        yield return new WaitForSeconds(delayBeforeIncrease);

        Debug.Log("게이지 증가 시작!");
        // 매 프레임마다 게이지 증가
        StartCoroutine(IncreaseGauge());
    }

    // 게이지를 점진적으로 증가시키는 코루틴
    System.Collections.IEnumerator IncreaseGauge()
    {
        while (gaugeImage.fillAmount < 1f)
        {
            gaugeImage.fillAmount += fillSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("게이지 최대치에 도달!");

        // 게이지가 최대치에 도달했을 때 Result_Success를 활성화
        resultSuccess.SetActive(true);

        // 5초 후에 메인 화면으로 이동
        yield return new WaitForSeconds(5f);

        // 메인 화면으로 이동
        SceneManager.LoadScene("Main Scene");
    }
}
