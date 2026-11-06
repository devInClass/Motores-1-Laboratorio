using System;
using System.Collections.Generic;
using SlugLab.Boss;
using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Enemies;
using SlugLab.Level;
using SlugLab.Player;
using SlugLab.UI;
using SlugLab.VFX;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SlugLab.EditorTools
{
    /// <summary>
    /// Genera todos los assets, prefabs y escenas del proyecto. Se puede lanzar
    /// desde el menú <c>Slug Lab</c> o en batchmode:
    /// <c>Unity.exe -batchmode -executeMethod SlugLab.EditorTools.SceneBuilder.BuildAll</c>.
    /// </summary>
    public static class SceneBuilder
    {
        private class GameAssets
        {
            public GameObject Player;
            public GameObject Soldier;
            public GameObject Grenadier;
            public GameObject Boss;
            public GameObject BulletPlayer;
            public GameObject BulletEnemy;
            public WeaponDefinition Weapon;
            public GameObject VfxChispas;
            public GameObject VfxExplosion;
            public Material[] FlashMaterials;
            public Material DissolveMaterial;
        }

        private class LevelSpec
        {
            public string Name;
            public int Width = 40;
            public (int start, int end)[] Gaps = Array.Empty<(int, int)>();
            public (int x, int y, int width, bool oneWay)[] Platforms = Array.Empty<(int, int, int, bool)>();
            public int[] Crates = Array.Empty<int>();
            public int[] Barrels = Array.Empty<int>();
            public Vector3 PlayerStart = new Vector3(2f, 1.2f, 0f);
            public int? CheckpointX;
            public int? GateX;
            public int? GoalX;
            public Vector3? SpawnerPosition;
            public int SpawnerCount;
            public Vector3? BossPosition;
            public string NextScene = string.Empty;
            public string Sign;
            public bool Hud = true;
            public bool Parallax = true;
            public bool Music = true;
            public Action<Vector3> Decorate;
        }

        [MenuItem("Slug Lab/3. Generar juego completo", priority = 3)]
        public static void BuildAll()
        {
            ProjectSetup.ConfigureAll();
            AssetDatabase.Refresh();

            BuildUtil.EnsureFolder(BuildUtil.PrefabRoot);
            BuildUtil.EnsureFolder(BuildUtil.DataRoot);
            BuildUtil.EnsureFolder(BuildUtil.ClipRoot);
            BuildUtil.EnsureFolder(BuildUtil.SceneRoot);

            GameAssets assets = CreateAssets();
            BuildMenu();
            BuildLevels(assets);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Slug Lab] Juego generado: 2 niveles, menú y 10 laboratorios.");
        }

        /// <summary>Entrada única para batchmode: genera todo y configura el build.</summary>
        public static void BuildEverything()
        {
            BuildAll();
            ConfigureBuildSettings();
        }

        // ------------------------------------------------------------------
        // Assets
        // ------------------------------------------------------------------

        private static GameAssets CreateAssets()
        {
            var assets = new GameAssets();
            BuildUtil.NewTile("tile_ground_top", BuildUtil.S("Tiles/tile_ground_top_0"), Tile.ColliderType.Grid);
            BuildUtil.NewTile("tile_ground_mid", BuildUtil.S("Tiles/tile_ground_mid_0"), Tile.ColliderType.Grid);
            AssetDatabase.SaveAssets();

            AnimatorController playerController = BuildPlayerAnimator();            AnimatorController soldierController = BuildSoldierAnimator();
            AnimatorController bossController = BuildBossAnimator();

            EnemyConfig soldierConfig = CreateEnemyConfig("Enemigo_Soldado", 3, 1.6f, 3.2f, 9f, 8f, 1.6f, 100);
            EnemyConfig grenadierConfig = CreateEnemyConfig("Enemigo_Granadero", 5, 1.3f, 2.7f, 10f, 9f, 2.1f, 150);

            assets.BulletPlayer = CreateBullet("Bala_Jugador", BuildUtil.S("VFX/bullet_player"), "PlayerAttack",
                LayerMask.GetMask("Ground", "Enemy"), 1, 20f);
            assets.BulletEnemy = CreateBullet("Bala_Enemigo", BuildUtil.S("VFX/bullet_enemy"), "EnemyAttack",
                LayerMask.GetMask("Ground", "Player"), 1, 11f);

            assets.Weapon = CreateWeapon(assets.BulletPlayer);
            assets.Player = CreatePlayerPrefab(playerController, assets.Weapon, assets.BulletPlayer);
            assets.Soldier = CreateEnemyPrefab("Enemigo_Soldado", soldierConfig, soldierController, assets.BulletEnemy, Color.white);
            assets.Grenadier = CreateEnemyPrefab("Enemigo_Granadero", grenadierConfig, soldierController, assets.BulletEnemy, new Color(0.72f, 0.82f, 1f));
            assets.Boss = CreateBossPrefab(bossController, assets.BulletEnemy, assets.Soldier);
            CreateVfx(assets);

            return assets;
        }

        private static AnimatorController BuildPlayerAnimator()
        {
            AnimatorController controller = BuildUtil.NewController("Player");
            controller.AddParameter("Speed01", AnimatorControllerParameterType.Float);
            controller.AddParameter("AimAngle", AnimatorControllerParameterType.Float);
            controller.AddParameter("Aiming", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("VerticalSpeed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Crouch", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Dead", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Hurt", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Melee", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine machine = controller.layers[0].stateMachine;

            AnimationClip idle = BuildUtil.Clip("player_idle", BuildUtil.Seq("Personajes", "player_idle_", 4), 8f);
            AnimationClip run = BuildUtil.Clip("player_run", BuildUtil.Seq("Personajes", "player_run_", 8), 14f);
            AnimationClip jump = BuildUtil.Clip("player_jump", new[] { BuildUtil.S("Personajes/player_jump_0") }, 1f, false);
            AnimationClip fall = BuildUtil.Clip("player_fall", new[] { BuildUtil.S("Personajes/player_fall_0") }, 1f, false);
            AnimationClip crouch = BuildUtil.Clip("player_crouch", new[] { BuildUtil.S("Personajes/player_crouch_0") }, 1f, false);
            AnimationClip hurt = BuildUtil.Clip("player_hurt", new[] { BuildUtil.S("Personajes/player_hurt_0") }, 1f, false);
            AnimationClip dead = BuildUtil.Clip("player_dead", new[] { BuildUtil.S("Personajes/player_dead_0") }, 1f, false);

            // ----------------------------------------------------------
            // Blend tree 1D: locomocion (idle <-> run) por velocidad 0..1
            // ----------------------------------------------------------
            AnimatorState locomotion = controller.CreateBlendTreeInController("Locomotion", out BlendTree locomotionTree);
            locomotionTree.blendType = BlendTreeType.Simple1D;
            locomotionTree.blendParameter = "Speed01";
            locomotionTree.useAutomaticThresholds = false;
            locomotionTree.AddChild(idle, 0f);
            locomotionTree.AddChild(run, 1f);
            machine.defaultState = locomotion;

            // ----------------------------------------------------------
            // Blend tree 2D: apuntado (5 direcciones x de pie/corriendo)
            // X = AimAngle (-90..90)   Y = Speed01 (0..1)
            // ----------------------------------------------------------
            AnimatorState aim = controller.CreateBlendTreeInController("Aim", out BlendTree aimTree);
            aimTree.blendType = BlendTreeType.FreeformCartesian2D;
            aimTree.blendParameter = "AimAngle";
            aimTree.blendParameterY = "Speed01";
            aimTree.useAutomaticThresholds = false;

            string[] directions = { "up", "up_right", "right", "down_right", "down" };
            float[] angles = { 90f, 45f, 0f, -45f, -90f };

            for (int i = 0; i < directions.Length; i++)
            {
                AnimationClip stand = BuildUtil.Clip($"player_aim_{directions[i]}_stand",
                    BuildUtil.Seq("Personajes", $"player_aim_{directions[i]}_stand_", 2), 10f);
                AnimationClip moving = BuildUtil.Clip($"player_aim_{directions[i]}_run",
                    BuildUtil.Seq("Personajes", $"player_aim_{directions[i]}_run_", 2), 12f);

                aimTree.AddChild(stand, new Vector2(angles[i], 0f));
                aimTree.AddChild(moving, new Vector2(angles[i], 1f));
            }

            AnimatorState jumpState = BuildUtil.State(machine, "Jump", jump);
            AnimatorState fallState = BuildUtil.State(machine, "Fall", fall);
            AnimatorState crouchState = BuildUtil.State(machine, "Crouch", crouch);
            AnimatorState hurtState = BuildUtil.State(machine, "Hurt", hurt);
            AnimatorState deadState = BuildUtil.State(machine, "Dead", dead);

            BuildUtil.Link(locomotion, aim, 0.08f, false,
                (AnimatorConditionMode.If, "Aiming", 0f),
                (AnimatorConditionMode.If, "Grounded", 0f),
                (AnimatorConditionMode.IfNot, "Crouch", 0f));
            BuildUtil.Link(aim, locomotion, 0.08f, false, (AnimatorConditionMode.IfNot, "Aiming", 0f));

            BuildUtil.Link(locomotion, jumpState, 0.05f, false,
                (AnimatorConditionMode.IfNot, "Grounded", 0f),
                (AnimatorConditionMode.Greater, "VerticalSpeed", 0.1f));
            BuildUtil.Link(aim, jumpState, 0.05f, false,
                (AnimatorConditionMode.IfNot, "Grounded", 0f),
                (AnimatorConditionMode.Greater, "VerticalSpeed", 0.1f));
            BuildUtil.Link(jumpState, fallState, 0.05f, false, (AnimatorConditionMode.Less, "VerticalSpeed", 0f));

            BuildUtil.Link(fallState, aim, 0.06f, false,
                (AnimatorConditionMode.If, "Grounded", 0f),
                (AnimatorConditionMode.If, "Aiming", 0f));
            BuildUtil.Link(fallState, locomotion, 0.06f, false,
                (AnimatorConditionMode.If, "Grounded", 0f),
                (AnimatorConditionMode.IfNot, "Aiming", 0f));

            BuildUtil.Link(locomotion, crouchState, 0.06f, false, (AnimatorConditionMode.If, "Crouch", 0f));
            BuildUtil.Link(aim, crouchState, 0.06f, false, (AnimatorConditionMode.If, "Crouch", 0f));
            BuildUtil.Link(crouchState, locomotion, 0.06f, false,
                (AnimatorConditionMode.IfNot, "Crouch", 0f),
                (AnimatorConditionMode.IfNot, "Aiming", 0f));
            BuildUtil.Link(crouchState, aim, 0.06f, false,
                (AnimatorConditionMode.IfNot, "Crouch", 0f),
                (AnimatorConditionMode.If, "Aiming", 0f));

            BuildUtil.AnyLink(machine, deadState, 0.05f, (AnimatorConditionMode.If, "Dead", 0f));
            BuildUtil.AnyLink(machine, hurtState, 0.03f, (AnimatorConditionMode.If, "Hurt", 0f));
            BuildUtil.Link(hurtState, locomotion, 0.06f, true);

            return controller;
        }

        private static AnimatorController BuildSoldierAnimator()
        {
            AnimatorController controller = BuildUtil.NewController("Soldier");
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Hurt", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Dead", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine machine = controller.layers[0].stateMachine;

            AnimationClip idle = BuildUtil.Clip("soldier_idle", BuildUtil.Seq("Enemigos", "soldier_idle_", 2), 4f);
            AnimationClip walk = BuildUtil.Clip("soldier_walk", BuildUtil.Seq("Enemigos", "soldier_walk_", 4), 10f);
            AnimationClip attack = BuildUtil.Clip("soldier_attack", new[] { BuildUtil.S("Enemigos/soldier_shoot_0") }, 1f, false);
            AnimationClip hurt = BuildUtil.Clip("soldier_hurt", new[] { BuildUtil.S("Enemigos/soldier_idle_0") }, 1f, false);
            AnimationClip dead = BuildUtil.Clip("soldier_dead", new[] { BuildUtil.S("Enemigos/soldier_dead_0") }, 1f, false);

            AnimatorState idleState = BuildUtil.State(machine, "Idle", idle, true);
            AnimatorState walkState = BuildUtil.State(machine, "Walk", walk);
            AnimatorState attackState = BuildUtil.State(machine, "Attack", attack);
            AnimatorState hurtState = BuildUtil.State(machine, "Hurt", hurt);
            AnimatorState deadState = BuildUtil.State(machine, "Dead", dead);

            BuildUtil.Link(idleState, walkState, 0.08f, false, (AnimatorConditionMode.Greater, "Speed", 0.1f));
            BuildUtil.Link(walkState, idleState, 0.08f, false, (AnimatorConditionMode.Less, "Speed", 0.1f));

            BuildUtil.AnyLink(machine, attackState, 0.03f, (AnimatorConditionMode.If, "Attack", 0f));
            BuildUtil.AnyLink(machine, hurtState, 0.03f, (AnimatorConditionMode.If, "Hurt", 0f));
            BuildUtil.AnyLink(machine, deadState, 0.03f, (AnimatorConditionMode.If, "Dead", 0f));

            BuildUtil.Link(attackState, idleState, 0.05f, true);
            BuildUtil.Link(hurtState, idleState, 0.05f, true);

            return controller;
        }

        private static AnimatorController BuildBossAnimator()
        {
            AnimatorController controller = BuildUtil.NewController("Boss");
            controller.AddParameter("Volley", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Charge", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Summon", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Phase", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Dead", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine machine = controller.layers[0].stateMachine;

            AnimationClip idle = BuildUtil.Clip("boss_idle", BuildUtil.Seq("Jefe", "boss_idle_", 2), 3f);
            AnimationClip attack = BuildUtil.Clip("boss_attack", new[] { BuildUtil.S("Jefe/boss_attack_0") }, 1f, false);
            AnimationClip dead = BuildUtil.Clip("boss_dead", new[] { BuildUtil.S("Jefe/boss_dead_0") }, 1f, false);

            AnimatorState idleState = BuildUtil.State(machine, "Idle", idle, true);
            AnimatorState attackState = BuildUtil.State(machine, "Attack", attack);
            AnimatorState deadState = BuildUtil.State(machine, "Dead", dead);

            BuildUtil.AnyLink(machine, attackState, 0.03f, (AnimatorConditionMode.If, "Volley", 0f));
            BuildUtil.AnyLink(machine, attackState, 0.03f, (AnimatorConditionMode.If, "Charge", 0f));
            BuildUtil.AnyLink(machine, attackState, 0.03f, (AnimatorConditionMode.If, "Summon", 0f));
            BuildUtil.AnyLink(machine, attackState, 0.03f, (AnimatorConditionMode.If, "Phase", 0f));
            BuildUtil.AnyLink(machine, deadState, 0.03f, (AnimatorConditionMode.If, "Dead", 0f));

            BuildUtil.Link(attackState, idleState, 0.05f, true);

            return controller;
        }

        private static EnemyConfig CreateEnemyConfig(string name, int health, float patrolSpeed, float chaseSpeed,
            float sight, float attackRange, float interval, int score)
        {
            string path = $"{BuildUtil.DataRoot}/{name}.asset";
            AssetDatabase.DeleteAsset(path);

            var config = ScriptableObject.CreateInstance<EnemyConfig>();
            config.maxHealth = health;
            config.patrolSpeed = patrolSpeed;
            config.chaseSpeed = chaseSpeed;
            config.sightRange = sight;
            config.attackRange = attackRange;
            config.attackInterval = interval;
            config.scoreValue = score;

            AssetDatabase.CreateAsset(config, path);
            return config;
        }

        private static WeaponDefinition CreateWeapon(GameObject bullet)
        {
            string path = $"{BuildUtil.DataRoot}/Arma_Ametralladora.asset";
            AssetDatabase.DeleteAsset(path);

            var weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            weapon.displayName = "AMETRALLADORA";
            weapon.damage = 1;
            weapon.projectilesPerShot = 1;
            weapon.fireInterval = 0.11f;
            weapon.magazineSize = 40;
            weapon.reloadTime = 1.15f;
            weapon.projectilePrefab = bullet.GetComponent<Projectile>();
            weapon.projectileSpeed = 20f;
            weapon.shotSfx = BuildUtil.A("SFX/disparo");
            weapon.screenShake = 0.04f;

            AssetDatabase.CreateAsset(weapon, path);
            return weapon;
        }

        private static GameObject CreateBullet(string name, Sprite sprite, string layer, LayerMask mask, int damage, float speed)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer(layer);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = "VFX";
            renderer.sortingOrder = 20;

            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.12f;

            var projectile = go.AddComponent<Projectile>();
            Binder.Set(projectile, "damage", damage);
            Binder.Set(projectile, "speed", speed);
            Binder.Set(projectile, "hitMask", mask);
            Binder.Set(projectile, "lifetime", 1.8f);

            return BuildUtil.SavePrefab(go, name);
        }

        private static GameObject CreatePlayerPrefab(AnimatorController controller, WeaponDefinition weapon, GameObject bullet)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.layer = LayerMask.NameToLayer("Player");

            var body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 4.2f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.62f, 1.12f);
            box.offset = new Vector2(0f, 0.56f);
            box.edgeRadius = 0.02f;

            var visual = new GameObject("Visual");
            visual.transform.SetParent(go.transform, false);

            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = BuildUtil.S("Personajes/player_idle_0");
            renderer.sortingLayerName = "Personajes";
            renderer.sortingOrder = 10;

            var animator = visual.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            var damageable = go.AddComponent<Damageable>();
            Binder.Set(damageable, "maxHealth", 5);
            Binder.Set(damageable, "team", Team.Player);
            Binder.Set(damageable, "invulnerabilityTime", 1f);
            Binder.Set(damageable, "knockbackForce", 6f);

            var input = go.AddComponent<PlayerInput>();
            var playerController = go.AddComponent<PlayerController>();
            var animatorBridge = go.AddComponent<PlayerAnimator>();
            var combat = go.AddComponent<PlayerCombat>();

            var groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(go.transform, false);
            groundCheck.transform.localPosition = new Vector3(0f, 0.05f, 0f);

            var melee = new GameObject("AtaqueCuerpo");
            melee.transform.SetParent(go.transform, false);
            melee.transform.localPosition = new Vector3(0.7f, 0.6f, 0f);
            var meleeBox = melee.AddComponent<BoxCollider2D>();
            meleeBox.isTrigger = true;
            meleeBox.size = new Vector2(0.8f, 0.6f);
            var hitbox = melee.AddComponent<Hitbox>();
            Binder.Set(hitbox, "damage", 2);
            Binder.Set(hitbox, "knockback", 7f);
            Binder.SetMask(hitbox, "targetMask", "Enemy");
            Binder.Set(hitbox, "activeTime", 0.12f);

            var pool = new GameObject("PoolBalas");
            pool.transform.SetParent(go.transform, false);
            var projectilePool = pool.AddComponent<ProjectilePool>();
            Binder.Set(projectilePool, "prefab", bullet.GetComponent<Projectile>());
            Binder.Set(projectilePool, "prewarm", 24);

            Binder.Set(playerController, "input", input);
            Binder.Set(playerController, "groundCheck", groundCheck.transform);
            Binder.Set(playerController, "bodyCollider", box);
            Binder.Set(playerController, "groundMask", (LayerMask)LayerMask.GetMask("Ground", "OneWayPlatform"));

            Binder.Set(animatorBridge, "animator", animator);
            Binder.Set(animatorBridge, "controller", playerController);
            Binder.Set(animatorBridge, "visual", visual.transform);

            Binder.Set(combat, "input", input);
            Binder.Set(combat, "controller", playerController);
            Binder.Set(combat, "animator", animatorBridge);
            Binder.Set(combat, "projectilePool", projectilePool);
            Binder.Set(combat, "meleeHitbox", hitbox);
            Binder.Set(combat, "startingWeapon", weapon);

            return BuildUtil.SavePrefab(go, "Player");
        }

        private static GameObject CreateEnemyPrefab(string name, EnemyConfig config, AnimatorController controller,
            GameObject bullet, Color tint)
        {
            var go = new GameObject(name);
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy");

            var body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 3.2f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.62f, 1.12f);
            box.offset = new Vector2(0f, 0.56f);

            var visual = new GameObject("Visual");
            visual.transform.SetParent(go.transform, false);

            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = BuildUtil.S("Enemigos/soldier_idle_0");
            renderer.color = tint;
            renderer.sortingLayerName = "Personajes";
            renderer.sortingOrder = 8;

            var animator = visual.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            var damageable = go.AddComponent<Damageable>();
            Binder.Set(damageable, "maxHealth", config.maxHealth);
            Binder.Set(damageable, "team", Team.Enemy);
            Binder.Set(damageable, "invulnerabilityTime", 0.25f);

            var eye = new GameObject("Ojo");
            eye.transform.SetParent(visual.transform, false);
            eye.transform.localPosition = new Vector3(0f, 0.95f, 0f);

            var probe = new GameObject("SondaSuelo");
            probe.transform.SetParent(go.transform, false);
            probe.transform.localPosition = new Vector3(0.3f, 0.05f, 0f);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(visual.transform, false);
            muzzle.transform.localPosition = new Vector3(0.45f, 0.8f, 0f);

            var pool = new GameObject("PoolBalas");
            pool.transform.SetParent(go.transform, false);
            var projectilePool = pool.AddComponent<ProjectilePool>();
            Binder.Set(projectilePool, "prefab", bullet.GetComponent<Projectile>());
            Binder.Set(projectilePool, "prewarm", 8);

            var gun = go.AddComponent<EnemyGun>();
            Binder.Set(gun, "projectilePool", projectilePool);
            Binder.Set(gun, "muzzle", muzzle.transform);
            Binder.Set(gun, "damage", 1);
            Binder.Set(gun, "projectileSpeed", 10f);
            Binder.Set(gun, "spreadDegrees", 4f);

            var brain = go.AddComponent<EnemyBrain>();
            Binder.Set(brain, "config", config);
            Binder.Set(brain, "eye", eye.transform);
            Binder.Set(brain, "groundProbe", probe.transform);
            Binder.Set(brain, "gun", gun);
            Binder.Set(brain, "animator", animator);
            Binder.Set(brain, "groundMask", (LayerMask)LayerMask.GetMask("Ground", "OneWayPlatform"));
            Binder.Set(brain, "playerMask", (LayerMask)LayerMask.GetMask("Player"));

            return BuildUtil.SavePrefab(go, name);
        }

        private static GameObject CreateBossPrefab(AnimatorController controller, GameObject bullet, GameObject minion)
        {
            var go = new GameObject("Jefe");
            go.tag = "Boss";
            go.layer = LayerMask.NameToLayer("Enemy");

            var body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 3.2f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(2.2f, 2.4f);
            box.offset = new Vector2(0f, 1.2f);

            var visual = new GameObject("Visual");
            visual.transform.SetParent(go.transform, false);

            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = BuildUtil.S("Jefe/boss_idle_0");
            renderer.sortingLayerName = "Personajes";
            renderer.sortingOrder = 6;

            var animator = visual.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            var damageable = go.AddComponent<Damageable>();
            Binder.Set(damageable, "maxHealth", 60);
            Binder.Set(damageable, "team", Team.Enemy);
            Binder.Set(damageable, "invulnerabilityTime", 0.15f);
            Binder.Set(damageable, "knockbackForce", 0f);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(go.transform, false);
            muzzle.transform.localPosition = new Vector3(1.4f, 1.5f, 0f);

            var summon = new GameObject("PuntoInvocacion");
            summon.transform.SetParent(go.transform, false);
            summon.transform.localPosition = new Vector3(-1.8f, 0.2f, 0f);

            var pool = new GameObject("PoolBalas");
            pool.transform.SetParent(go.transform, false);
            var projectilePool = pool.AddComponent<ProjectilePool>();
            Binder.Set(projectilePool, "prefab", bullet.GetComponent<Projectile>());
            Binder.Set(projectilePool, "prewarm", 40);

            var boss = go.AddComponent<BossController>();
            Binder.Set(boss, "projectilePool", projectilePool);
            Binder.Set(boss, "muzzle", muzzle.transform);
            Binder.Set(boss, "summonPoint", summon.transform);
            Binder.Set(boss, "minionPrefab", minion);
            Binder.Set(boss, "groundMask", (LayerMask)LayerMask.GetMask("Ground"));

            return BuildUtil.SavePrefab(go, "Jefe");
        }

        // ------------------------------------------------------------------
        // Escenas
        // ------------------------------------------------------------------

        // ------------------------------------------------------------------
        // VFX y shaders
        // ------------------------------------------------------------------

        private static void CreateVfx(GameAssets assets)
        {
            BuildUtil.EnsureFolder($"{BuildUtil.DataRoot}/Materiales");

            Shader flashShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/_SlugLab/Shaders/SpriteFlash.shader");
            if (flashShader == null) flashShader = Shader.Find("SlugLab/2D Sprite Flash");
            if (flashShader == null)
            {
                Debug.LogError("[Slug Lab] No se encontro el shader SpriteFlash.");
                return;
            }

            assets.FlashMaterials = new[]
            {
                CreateFlashMaterial("Blanco_Flash_00", flashShader, 0f, 0f),
                CreateFlashMaterial("Blanco_Flash_50", flashShader, 0.5f, 0f),
                CreateFlashMaterial("Blanco_Flash_100", flashShader, 1f, 0f)
            };
            assets.DissolveMaterial = CreateFlashMaterial("Disolucion_55", flashShader, 0f, 0.55f);

            Material particleMaterial = CreateParticleMaterial();
            assets.VfxChispas = CreateParticlePrefab("VFX_Chispas", particleMaterial, 14, 0.35f, 5f, 0.16f, new Color(1f, 0.78f, 0.3f), 2.2f);
            assets.VfxExplosion = CreateParticlePrefab("VFX_Explosion", particleMaterial, 30, 0.65f, 3.5f, 0.35f, new Color(1f, 0.45f, 0.15f), 3f);
        }

        private static Material CreateFlashMaterial(string name, Shader shader, float flash, float dissolve)
        {
            string path = $"{BuildUtil.DataRoot}/Materiales/{name}.mat";
            AssetDatabase.DeleteAsset(path);

            var material = new Material(shader) { name = name };
            material.SetFloat("_FlashAmount", flash);
            material.SetFloat("_DissolveAmount", dissolve);

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Material CreateParticleMaterial()
        {
            string path = $"{BuildUtil.DataRoot}/Materiales/Particula_Chispa.mat";
            AssetDatabase.DeleteAsset(path);

            var material = new Material(Shader.Find("Sprites/Default")) { name = "Particula_Chispa" };
            Sprite spark = BuildUtil.S("VFX/impact_0");
            if (spark != null) material.mainTexture = spark.texture;

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static GameObject CreateParticlePrefab(string name, Material material, int burst, float lifetime,
            float speed, float size, Color color, float destroyDelay)
        {
            var go = new GameObject(name);
            var particles = go.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = particles.main;
            main.duration = 0.4f;
            main.loop = false;
            main.playOnAwake = true;
            main.startLifetime = lifetime;
            main.startSpeed = speed;
            main.startSize = size;
            main.startColor = color;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 120;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)burst) });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.08f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.sortingLayerName = "VFX";
            renderer.sortingOrder = 30;

            var destroy = go.AddComponent<VFXAutoDestroy>();
            Binder.Set(destroy, "delay", destroyDelay);

            return BuildUtil.SavePrefab(go, name);
        }

        private static void BuildMenu()
        {
            var scene = BuildUtil.NewScene();
            BuildUtil.CreateAudio(true);
            BuildUtil.CreateBackdrop(24, false);
            BuildUtil.CreateCamera(new Vector2(0f, 4f), new Vector2(0f, 4f));

            Canvas canvas = BuildUtil.CreateCanvas("Menu");
            BuildUtil.CreateEventSystem();

            BuildUtil.NewImage(canvas.transform, "Veladura", null, new Color(0.02f, 0.03f, 0.06f, 0.55f));
            var veil = canvas.transform.Find("Veladura").GetComponent<Image>();
            BuildUtil.Place(veil, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            Text title = BuildUtil.NewText(canvas.transform, "Titulo", "SLUG LAB", 150, TextAnchor.MiddleCenter, new Color(0.95f, 0.95f, 0.75f));
            BuildUtil.Place(title, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 260f), new Vector2(1400f, 180f));

            Text subtitle = BuildUtil.NewText(canvas.transform, "Subtitulo",
                "MOTORES DE VIDEOJUEGOS I  -  OPERACION CANTINA", 40, TextAnchor.MiddleCenter, new Color(0.75f, 0.88f, 0.9f));
            BuildUtil.Place(subtitle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(1400f, 60f));

            Button play = BuildUtil.NewButton(canvas.transform, "Boton_Jugar", "JUGAR", Color.white, out _);
            BuildUtil.Place(play, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(440f, 90f));

            Button quit = BuildUtil.NewButton(canvas.transform, "Boton_Salir", "SALIR", Color.white, out _);
            BuildUtil.Place(quit, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(440f, 90f));

            Text keys = BuildUtil.NewText(canvas.transform, "Controles",
                "A/D o flechas: moverse   ESPACIO: saltar   J o clic: disparar   K o clic derecho: cuchillo   S: agacharse   ESC: pausa",
                26, TextAnchor.MiddleCenter, new Color(0.8f, 0.85f, 0.9f));
            BuildUtil.Place(keys, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(1600f, 40f));

            var menu = canvas.gameObject.AddComponent<MenuController>();
            Binder.Set(menu, "playButton", play);
            Binder.Set(menu, "quitButton", quit);

            BuildUtil.SaveScene(scene, "Menu");
        }

        private static void BuildLevels(GameAssets assets)
        {
            BuildLevel(assets, new LevelSpec
            {
                Name = "Nivel1_Cantina",
                Width = 84,
                Gaps = new[] { (20, 22), (38, 40), (58, 59) },
                Platforms = new[]
                {
                    (13, 2, 4, true), (27, 2, 5, true), (44, 2, 4, true),
                    (51, 3, 4, true), (64, 2, 4, true)
                },
                Crates = new[] { 8, 33, 61 },
                Barrels = new[] { 12, 47, 55 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                CheckpointX = 34,
                GateX = 70,
                SpawnerPosition = new Vector3(66f, 0.6f, 0f),
                SpawnerCount = 4,
                GoalX = 79,
                NextScene = "Nivel2_Jefe",
                Sign = "NIVEL 1 - LA CANTINA\n\nA/D: moverte   ESPACIO: saltar\nJ: disparar   K: cuchillo   ESC: pausa\n\nObjetivo: llega a la puerta del fondo."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Nivel2_Jefe",
                Width = 36,
                PlayerStart = new Vector3(4f, 1.2f, 0f),
                CheckpointX = 5,
                BossPosition = new Vector3(25f, 0.5f, 0f),
                NextScene = string.Empty,
                Sign = "NIVEL 2 - EL JEFE\n\nEsquiva las rafagas y dispara cuando\ntermine su ataque. Tiene 3 fases."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_01_Sprites",
                Width = 34,
                Hud = false,
                Parallax = false,
                Music = false,
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                Sign = "LAB 01 - SPRITES\n\nObserva estas imagenes: todas deberian\nmedir lo mismo en unidades. Compara el\nplayer (1x1.25), el enemigo (1x1.25) y el jefe (3x3)."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_02_Animacion",
                Width = 30,
                Hud = false,
                Sign = "LAB 02 - ANIMACION\n\nAbre el Animator del Player y revisa\nlos estados Idle, Run, Jump y Fall.\nPrueba cambiar la velocidad de Run."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_03_Fisica",
                Width = 52,
                Gaps = new[] { (14, 17), (30, 33) },
                Platforms = new[] { (10, 2, 3, true), (22, 3, 4, true), (38, 2, 4, true) },
                Crates = new[] { 6 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                CheckpointX = 26,
                GoalX = 47,
                Sign = "LAB 03 - FISICA 2D\n\nCruza los huecos usando coyote time\ny salto variable. Manten el salto\npara llegar mas alto."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_04_Combate",
                Width = 40,
                Platforms = new[] { (12, 2, 3, true), (28, 2, 3, true) },
                Crates = new[] { 9, 9, 33 },
                Barrels = new[] { 18, 22 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                SpawnerPosition = new Vector3(30f, 0.6f, 0f),
                SpawnerCount = 3,
                GoalX = 37,
                Sign = "LAB 04 - COMBATE\n\nDispara (J) y usa el cuchillo (K).\nFijate en las capas PlayerAttack y\nEnemyAttack: por eso no te das a ti mismo."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_05_IA",
                Width = 46,
                Platforms = new[] { (10, 2, 3, true), (20, 3, 3, true), (31, 2, 3, true) },
                Crates = new[] { 15, 27 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                SpawnerPosition = new Vector3(38f, 0.6f, 0f),
                SpawnerCount = 3,
                GoalX = 43,
                Sign = "LAB 05 - IA DE ENEMIGOS\n\nLos enemigos patrullan, te detectan\npor linea de vision y te persiguen.\nPrueba esconderte detras de una caja."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_06_Niveles",
                Width = 64,
                Gaps = new[] { (24, 26) },
                Platforms = new[] { (18, 2, 4, true), (34, 2, 4, true), (44, 3, 4, true) },
                Crates = new[] { 12, 40 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                CheckpointX = 30,
                GateX = 52,
                SpawnerPosition = new Vector3(48f, 0.6f, 0f),
                SpawnerCount = 3,
                GoalX = 60,
                Sign = "LAB 06 - NIVELES\n\nCamara con limites, parallax de fondo,\ncheckpoint y puerta que se abre al\nlimpiar la oleada."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_07_UI",
                Width = 30,
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                Sign = "LAB 07 - INTERFAZ Y FLUJO\n\nPrueba el HUD, la pausa (ESC o boton II),\nla muerte y la pantalla final.\nLos controles tactiles se prueban aqui en el editor.",
                Decorate = _ =>
                {
                    var go = new GameObject("ControlesMovilesDemo");
                    var mobile = go.AddComponent<MobileControls>();
                    Binder.Set(mobile, "forceOnDesktop", true);
                }
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_08_Jefe",
                Width = 34,
                PlayerStart = new Vector3(4f, 1.2f, 0f),
                BossPosition = new Vector3(24f, 0.5f, 0f),
                NextScene = string.Empty,
                Sign = "LAB 08 - JEFE FINAL\n\nEl jefe tiene 3 fases (60, 40 y 20 de vida).\nAjusta sus patrones en el inspector."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_09_Bugs",
                Width = 44,
                Platforms = new[] { (15, 2, 4, true) },
                Crates = new[] { 10, 32 },
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                SpawnerPosition = new Vector3(30f, 0.6f, 0f),
                SpawnerCount = 2,
                GoalX = 40,
                Sign = "LAB 09 - CACERIA DE BUGS\n\nAbre Assets/_Curso/Bugs, activa el define\nBUG_HUNT y corrige los 4 scripts hasta\nque sus tests pasen."
            });

            BuildLevel(assets, new LevelSpec
            {
                Name = "Lab_10_VFX",
                Width = 34,
                PlayerStart = new Vector3(2f, 1.2f, 0f),
                Sign = "LAB 10 - VFX Y SHADERS\n\nLos tres soldados usan el mismo shader con\ndistinto Flash Amount. El granadero tiene\nDissolve Amount. Las particulas son VFX.\nPrueba a cambiar los valores en el material.",
                Decorate = _ =>
                {
                    for (int i = 0; i < assets.FlashMaterials.Length; i++)
                    {
                        var dummy = BuildUtil.NewSprite($"Blanco_{i}", BuildUtil.S("Enemigos/soldier_idle_0"), "Enemy", "Personajes", 6);
                        dummy.transform.position = new Vector3(9f + i * 3f, 0.5f, 0f);
                        dummy.GetComponent<SpriteRenderer>().sharedMaterial = assets.FlashMaterials[i];

                        var box = dummy.AddComponent<BoxCollider2D>();
                        box.size = new Vector2(0.7f, 1.2f);
                        box.offset = new Vector2(0f, 0.6f);
                    }

                    var dissolve = BuildUtil.NewSprite("Disolucion", BuildUtil.S("Enemigos/grenadier_idle_0"), "Enemy", "Personajes", 6);
                    dissolve.transform.position = new Vector3(19f, 0.5f, 0f);
                    dissolve.GetComponent<SpriteRenderer>().sharedMaterial = assets.DissolveMaterial;

                    BuildUtil.Place(assets.VfxChispas, new Vector3(23f, 1f, 0f));
                    BuildUtil.Place(assets.VfxExplosion, new Vector3(27f, 1.2f, 0f));
                }
            });
        }

        private static void BuildLevel(GameAssets assets, LevelSpec spec)
        {
            var scene = BuildUtil.NewScene();

            BuildUtil.CreateAudio(spec.Music);
            BuildUtil.CreateBackdrop(spec.Width, spec.Parallax);

            float halfWidth = BuildUtil.CameraSize * 16f / 9f;
            Vector2 min = new Vector2(Mathf.Min(halfWidth, spec.Width * 0.5f), 4f);
            Vector2 max = new Vector2(Mathf.Max(spec.Width - halfWidth, halfWidth), 4f);
            BuildUtil.CreateCamera(min, max, 0f, spec.Width);

            BuildUtil.CreateGround(spec.Width, spec.Gaps);

            foreach ((int x, int y, int width, bool oneWay) in spec.Platforms)
            {
                BuildUtil.CreatePlatform($"Plataforma_{x}_{y}", new Vector3(x + width * 0.5f, y, 0f), width, oneWay);
            }

            foreach (int x in spec.Crates)
            {
                BuildUtil.CreateProp("Caja", BuildUtil.S("Props/crate"), new Vector3(x, 0.6f, 0f));
            }

            foreach (int x in spec.Barrels)
            {
                BuildUtil.CreateProp("Barril", BuildUtil.S("Props/barrel"), new Vector3(x, 0.6f, 0f));
            }

            GameObject player = BuildUtil.Place(assets.Player, spec.PlayerStart);
            var playerController = player.GetComponent<PlayerController>();
            var combat = player.GetComponent<PlayerCombat>();

            GameObject bossObject = null;
            BossController boss = null;
            if (spec.BossPosition.HasValue)
            {
                bossObject = BuildUtil.Place(assets.Boss, spec.BossPosition.Value);
                boss = bossObject.GetComponent<BossController>();
            }

            BuildUtil.CreateFlow(spec.NextScene);

            if (spec.Hud) BuildUtil.CreateHud(playerController, combat, boss);

            if (spec.CheckpointX.HasValue) BuildUtil.CreateCheckpoint(new Vector3(spec.CheckpointX.Value, 0.6f, 0f));

            LevelGate gate = null;
            if (spec.GateX.HasValue) gate = BuildUtil.CreateGate(new Vector3(spec.GateX.Value, 0.6f, 0f));

            if (spec.SpawnerPosition.HasValue)
            {
                BuildUtil.CreateSpawner(spec.SpawnerPosition.Value, assets.Soldier, spec.SpawnerCount, 2.5f, gate);
            }

            if (spec.GoalX.HasValue) BuildUtil.CreateGoal(new Vector3(spec.GoalX.Value, 0.6f, 0f));

            BuildUtil.CreateKillZone(spec.Width, -7f);

            if (!string.IsNullOrEmpty(spec.Sign))
            {
                BuildUtil.AddInstructionSign(new Vector3(spec.PlayerStart.x, 2.6f, 0f), spec.Sign);
            }

            spec.Decorate?.Invoke(spec.PlayerStart);

            BuildUtil.SaveScene(scene, spec.Name);
        }

        // ------------------------------------------------------------------
        // Build settings
        // ------------------------------------------------------------------

        [MenuItem("Slug Lab/4. Configurar escenas de compilacion", priority = 4)]
        public static void ConfigureBuildSettings()
        {
            string[] order =
            {
                "Menu", "Nivel1_Cantina", "Nivel2_Jefe",
                "Lab_01_Sprites", "Lab_02_Animacion", "Lab_03_Fisica", "Lab_04_Combate",
                "Lab_05_IA", "Lab_06_Niveles", "Lab_07_UI", "Lab_08_Jefe", "Lab_09_Bugs", "Lab_10_VFX"
            };

            var list = new List<EditorBuildSettingsScene>();
            foreach (string name in order)
            {
                string path = $"{BuildUtil.SceneRoot}/{name}.unity";
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
                {
                    list.Add(new EditorBuildSettingsScene(path, true));
                }
            }

            EditorBuildSettings.scenes = list.ToArray();
            Debug.Log($"[Slug Lab] {list.Count} escenas en Build Settings.");
        }
    }
}