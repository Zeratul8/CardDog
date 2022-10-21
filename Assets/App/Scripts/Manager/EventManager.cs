using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{
    public delegate void OnEvent(params object[] param);
    
    static Dictionary<string, List<OnEvent>> eventDic = new Dictionary<string, List<OnEvent>>();
    public static void AddListener(string key, OnEvent callback){
        if(string.IsNullOrEmpty(key)){
            Debug.Log("Key를 다시 확인해주세요.");
            return;
        }
        if(!eventDic.ContainsKey(key)){
            eventDic.Add(key, new List<OnEvent>());
            eventDic[key].Add(callback);
        }
        else {
            if(eventDic[key].Contains(callback)) {
                Debug.Log("이미 등록된 함수입니다.");
                return;
            }
            eventDic[key].Add(callback);
        }
    }
    public static void RemoveListener(string key, OnEvent action){
        if(eventDic.ContainsKey(key)){
            if(eventDic[key].Contains(action)){
                eventDic[key].Remove(action);
            }
            else {
                Debug.Log("등록된 적이 없거나 이미 삭제된 함수입니다.");
                return;
            }
        }
        else{
            Debug.Log("Key를 다시 확인해주세요.");
            return;
        }
    }
    public static void CallEvent(string key, params object[] param){
        if(string.IsNullOrEmpty(key)){
            Debug.Log("Key를 다시 확인해주세요.");
            return;
        }
        else{
            if(!eventDic.ContainsKey(key)){
                Debug.Log("Key를 다시 확인해주세요.");
            return;
            }
            else{
                foreach(OnEvent e in eventDic[key]){
                    e.Invoke(param);
                }
            }
        }
    }
}
