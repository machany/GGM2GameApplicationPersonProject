using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Work.Scripts.Executors
{
    [DefaultExecutionOrder(-10)]
    // 명령어의 메인 컨트롤러
    public class CommandExecutor : MonoBehaviour // 유효성 검사, 명령의 대상 받기, 명령어 실행, 최소한의 파싱
    {
        public class ReturnValue
        {
            public bool isVoid;
            public object value;
        }

        [Header("Default Setting")]
        [SerializeField] private ObjectFinderSO objectManagerFinder;

        public Action<string> OnErrorEvent;

        private MethodArchive _methodArchive;
        private ObjectManager _objetManager;

        private void Awake()
        {
            _methodArchive = new MethodArchive();
            _methodArchive.ArchiveAllMethod();

            _objetManager = objectManagerFinder.GetObject<ObjectManager>();

            // 오브젝트 이름이 명령어랑 같아질 수 없게
            foreach (string method in _methodArchive.GetMethodNames())
                _objetManager.commandKeywords.Add(method);
        }

        private void Start()
        {
            _objetManager = objectManagerFinder.GetObject<ObjectManager>();
        }

        public ReturnValue ExecuteCommand(string script, IScriptable scriptable)
        {
            if (string.IsNullOrEmpty(script) || scriptable == null)
                return null;

            string[] commands = script.Split(' ');

            List<string> parameters = commands.ToList();

            string command = parameters[0];
            parameters.RemoveAt(0);

            for (int i = 0; i < parameters.Count; ++i)
                if (EqualityComparer<string>.Default.Equals(parameters[i], _objetManager.objectCallMeKeyword))
                    parameters[i] = scriptable.ObjectName;

            // 참조로 인한 변경 방지
            ReturnValue returnValue = InvokeCommand(command, parameters.ToArray());
            return returnValue;
        }

        public ReturnValue InvokeCommand(string command, params string[] parameters)
        {
            if (parameters == null)
                return null;

            // 인자가 없을 때
            if (parameters.Length <= 0)
                return Invoke("Commander", command);

            // 실행 대상이 없을 때
            if (!_objetManager.TryGetObject(parameters[0], out IScriptable scriptable))
                // 커맨더 실행자들은 if같은 제어문일 가능성이 높음. 즉 if(명령어)같은 꼴로 명령어에 명령어가 들어가는 상황이 일어남.
                // 이와 같은 이유로 오브젝트 플레이어가 아닌 경우는 string[]자체를 넘겨 처리하도록함.
                return Invoke("Commander", command, new object[] { parameters });

            // 실행 대상이 포함 된 인수에서 실행 대상에 해당하는 0번째를 제거
            List<object> param = parameters.ToList().Select(v => (object)v).ToList();
            param.RemoveAt(0);
            param.Insert(0, scriptable);

            // 만약, 실행 대상을 제외한 인수가 없을 때
            if (parameters.Length <= 0)
                return Invoke(scriptable.ObjectName, command, scriptable);
            else
            {
                // 배열로 넘기는 방식이 아닌, 하나하나 넘기는 방식은 없는 듯
                // 굳이 꼭 해야하면 동덕으로 델리케이트 만들어서 parameter수 만큼 동적으로 만드는 방식밖에 못 하는 듯
                return Invoke(scriptable.ObjectName, command, param.ToArray());
            }
        }

        public ReturnValue Invoke(string errorOwner, string command, params object[] parameters)
        {
            try
            {
                ReturnValue returnValue = new ReturnValue();

                if (parameters == null || parameters.Length <= 0)
                    returnValue.value = _methodArchive.Invoke(command);
                else
                    returnValue.value = _methodArchive.Invoke(command, parameters);

                returnValue.isVoid = _methodArchive.GetMethod(command).Method.ReturnType == typeof(void);
                return returnValue;
            }
            catch (MethodArchiveException ex)
            {
                OnErrorEvent?.Invoke(errorOwner);
                Debug.LogError($"{errorOwner} : {ex.message}\n{ex.exception.Message}");
                return null;
            }
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

            InvokeCommand(command, paremeters);
        }
#endif
    }
}
