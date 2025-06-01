using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    [DefaultExecutionOrder(-100)] // 절대적으로 우선하여
    public class CommandExecutor : MonoBehaviour // 유효성 검사, 명령의 대상 받기, 명령어 실행
    {
        [SerializeField] private ObjectFinder objectManagerFinder;
        [SerializeField] private ObjectManager objetManager;

        public Action<string> OnErrorEvent;
        private MethodArchive methodArchive;

        private void Awake()
        {
            methodArchive = new MethodArchive();
            methodArchive.ArchiveAllMethod();
            objetManager = objectManagerFinder.GetObject<ObjectManager>();
        }

        public bool TryInvoke(string command, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                return false;

            if (!objetManager.TryGetObject(parameters[0], out IScriptable scriptable))
                return false;

            try
            {
                if (parameters.Length <= 1)
                    methodArchive.Invoke(command, scriptable);
                else
                {
                    List<string> li = parameters.ToList();
                    li.RemoveAt(0);
                    methodArchive.Invoke(command, scriptable, li.ToArray());
                }
                // 배열로 넘기는 방식이 아닌, 하나하나 넘기는 방식은 없는 듯
                // 굳이 꼭 해야하면 동덕으로 델리케이트 만들어서 parameter수 만큼 동적으로 만드는 방식밖에 못 하는 듯
            }
            catch (MethodArchiveException ex)
            {
                OnErrorEvent?.Invoke(scriptable.Object.name);
                Debug.Log(ex.message);
            }

            return true;
        }

#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private string command;
        [SerializeField] private string paremeter;

        [ContextMenu("Invoke Command")]
        private void TestInvokeCommand()
        {
            if (string.IsNullOrEmpty(command) || string.IsNullOrEmpty(paremeter))
                return;

            string[] paremeters = paremeter.Split(' ');

            TryInvoke(command, paremeters);
        }
#endif
    }
}
