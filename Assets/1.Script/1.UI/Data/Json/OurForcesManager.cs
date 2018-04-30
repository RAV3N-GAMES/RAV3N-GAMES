using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;


/*
 * Tbl_OurForceSetup Index
 * 0 ~ 99 : Gaurd
 * 100 ~ 199 : QuickReactionForce
 * 200 ~ 299 : BiochemistryUnit
 * 300 ~ 399 : Researcher
 */
/*
 * OurForces Property List
   public int HP : 총 HP(Player 레벨(Fame) 비례)
   public int Attack : 평타 공격력 
   public int SkillCool : 스킬쿨(스킬 사용을 위해 평타를 쳐야 하는 횟수)
   public int Price : 배치 시 가격
   public int UpgradeCost : 업그레이드 가격
   public double HealCost : 체력 1 당 회복 비용
   public int ActiveCost : 잠금해제 비용(기본 제공은 0)
   public double AttackRange : 평타 사거리(반지름)
   public double AttackAngle : 평타 공격 범위
   public int HitConstrain : 평타 1회당 타격 가능한 적 최대 수
   public double EnemyRecognizeRangeHalf : 적 인식 범위(Sphere Collider 기준 반지름)
   public double MoveSpeed : 이동속도. 1 = 1초에 타일 1개 이동
   public double AttackPeriod : 공격속도
   public double SkillDuration : 스킬 지속시간
   public double SkillCoefficient_Damage : 스킬 데미지 계수. 미리 계산한 값을 저장 ex) 데미지가 10인 평타의 1.2배 => 12 저장. 
   public double SkillCoefficient_Util : 스킬 유틸 계수. 이동속도, 공격 범위 증가 등에 활용
   public double SkillCoefficient_Defence : 스킬 방어 계수. 아직 관련 스킬은 없음.
   public double SkillRange : 스킬 범위. ******** 방 전체인 경우 -1 로 표기 ***********
 */

public class OurForcesManager : JsonReadWrite
{
    const string Path1 = "Data/OurForces_Guard";
    const string Path2 = "Data/OurForces_QuickReactionForces";
    const string Path3 = "Data/OurForces_BiochemistryUnit";
    const string Path4 = "Data/OurForces_Researcher";
    public static List<OurForcesObject> Tbl_OurForceSetup = new List<OurForcesObject>();

    // Use this for initialization
    void Awake()
    {
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
        /* 입력값 확인용 코드입니다. 지우지 마세요.
                for (int i = 0; i < 400; i++)
                {
                    Debug.Log(i + "th index: " + Tbl_OurForceSetup[i].Level+" " + Tbl_OurForceSetup[i].HP + " " + Tbl_OurForceSetup[i].Attack + " " + Tbl_OurForceSetup[i].SkillCool + " " + Tbl_OurForceSetup[i].Price + " " + Tbl_OurForceSetup[i].UpgradeCost+ " " + Tbl_OurForceSetup[i].HealCost + " " + Tbl_OurForceSetup[i].id + " " + Tbl_OurForceSetup[i].Type + " " + Tbl_OurForceSetup[i].ActiveCost + " " + Tbl_OurForceSetup[i].AttackRange + " " + Tbl_OurForceSetup[i].AttackAngle + " " + Tbl_OurForceSetup[i].HitConstrain+ " " + Tbl_OurForceSetup[i].AttackCool+ " " + Tbl_OurForceSetup[i].EnemyRecognizeRangeHalf + " " + Tbl_OurForceSetup[i].MoveSpeed + " ");
                }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            OurForcesObject tmp = new OurForcesObject();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.HP = int.Parse(data[i]["HP"].ToString());
            tmp.Attack = int.Parse(data[i]["Attack"].ToString());
            tmp.SkillCool = int.Parse(data[i]["SkillCool"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.HealCost = double.Parse(data[i]["HealCost"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            tmp.ActiveCost = int.Parse(data[i]["ActiveCost"].ToString());
            tmp.AttackRange = double.Parse(data[i]["AttackRange"].ToString());
            tmp.AttackAngle = double.Parse(data[i]["AttackAngle"].ToString());
            tmp.HitConstrain = int.Parse(data[i]["HitConstrain"].ToString());
            tmp.EnemyRecognizeRangeHalf = double.Parse(data[i]["EnemyRecognizeRangeHalf"].ToString());
            tmp.MoveSpeed = double.Parse(data[i]["MoveSpeed"].ToString());
            tmp.AttackPeriod = double.Parse(data[i]["AttackPeriod"].ToString());
            tmp.SkillDuration = double.Parse(data[i]["SkillDuration"].ToString());
            tmp.SkillCoefficient_Damage = double.Parse(data[i]["SkillCoefficient_Damage"].ToString());
            tmp.SkillCoefficient_Util = double.Parse(data[i]["SkillCoefficient_Util"].ToString());
            tmp.SkillCoefficient_Defence = double.Parse(data[i]["SkillCoefficient_Defence"].ToString());
            tmp.SkillRange = double.Parse(data[i]["SkillRange"].ToString());
            Tbl_OurForceSetup.Add(tmp);
        }
    }
}
