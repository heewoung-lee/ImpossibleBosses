using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Anim_Hash
{

    public static int Idle { get; private set; } = Animator.StringToHash("Idle");
    public static int Run { get; private set; } = Animator.StringToHash("Run");
    public static int Die { get; private set; } = Animator.StringToHash("Die");
    public static int Attack { get; private set; } = Animator.StringToHash("Attack");
}
public class Enemy_Anim_Hash
{
    public static int Golem_Wait { get; private set; } = Animator.StringToHash("Golem_Wait");
    public static int Golem_Idle { get; private set; } = Animator.StringToHash("Golem_Idle");
    public static int Golem_Rise { get; private set; } = Animator.StringToHash("Golem_Rise");
    public static int Golem_Walk { get; private set; } = Animator.StringToHash("Golem_Walk");
    public static int Golem_Attack1 { get; private set; } = Animator.StringToHash("Golem_Attack1");
    public static int Golem_Attack2 { get; private set; } = Animator.StringToHash("Golem_Attack2");
    public static int Golem_Skill { get; private set; } = Animator.StringToHash("Golem_Skill");
    public static int Golem_Attacked { get; private set; } = Animator.StringToHash("Golem_Attacked");
    public static int Golem_Dead { get; private set; } = Animator.StringToHash("Golem_Dead");

}