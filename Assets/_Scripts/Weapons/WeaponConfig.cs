using UnityEngine;
using RPGDemo.Items;
using Core.AudioSystem;
namespace RPGDemo.Weapons
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipmentConfig
    {
        [SerializeField] private float weaponDamage;
        [SerializeField] GameObject equippedPrefab;
        [SerializeField] private bool isUnarmed = false;
        [SerializeField] private LayerMask weaponLayer;
        [SerializeField] SoundData hitSoundData;
        [SerializeField] SoundData attackSoundData;
        [SerializeField] ParticleSystem slashVfx;

        // [field: SerializeField] public int MaxCombo { get; private set; }

        public float WeaponDamage => weaponDamage;
        public SoundData HitSound => hitSoundData;
        public SoundData AttackSound => attackSoundData;
        public ParticleSystem SlashVfx => slashVfx;


        public string[] AttackCombo;
        public int MaxCombo => AttackCombo.Length;


        public bool CanAttack()
        {
            return MaxCombo > 0;
        }

        public GameObject GetEquippedPrefab() => equippedPrefab;

        //todo:对不同的武器使用不同的动画覆盖器

        public GameObject SpawnWeapon(Transform holder)
        {
            if (equippedPrefab == null) return null;

            GameObject weapon = Instantiate(equippedPrefab, holder);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            foreach (Transform child in weapon.transform)
            {
                child.gameObject.layer = (int)Mathf.Log(weaponLayer.value, 2);
            }



            return weapon;

        }

        public string GetNextCombo(int currentCombo)
        {
            if (MaxCombo <= 0) return "";
            int nextCombo = (currentCombo + 1) % MaxCombo;
            return AttackCombo[nextCombo];
        }

        public string GetRandomAttack()
        {

            if (AttackCombo == null || AttackCombo.Length == 0) return string.Empty;
            int random = Random.Range(0, AttackCombo.Length);
            return AttackCombo[random];
        }


    }
}
