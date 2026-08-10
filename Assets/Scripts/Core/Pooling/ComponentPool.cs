using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ComponentPool<T> : IDisposable where T : Component
{
    private readonly T prefab;
    private readonly Queue<T> availableInstances = new();
    private readonly int capacity;

    private int createdCount;
    private bool isDisposed;

    internal ComponentPool(T prefab, int capacity)
    {
        this.prefab = prefab;
        this.capacity = Mathf.Max(capacity, 1);
    }

    internal bool CanGet =>
        !isDisposed &&
        prefab != null &&
        (availableInstances.Count > 0 || createdCount < capacity);

    internal T Get()
    {
        if (isDisposed || prefab == null)
            return null;

        while (availableInstances.Count > 0)
        {
            T instance = availableInstances.Dequeue();
            if (instance != null)
                return instance;

            createdCount--;
        }

        if (createdCount >= capacity)
            return null;

        T newInstance = UnityEngine.Object.Instantiate(prefab);
        newInstance.gameObject.SetActive(false);
        createdCount++;
        return newInstance;
    }

    internal void Release(T instance)
    {
        if (instance == null) return;

        if (isDisposed)
        {
            UnityEngine.Object.Destroy(instance.gameObject);
            return;
        }

        availableInstances.Enqueue(instance);
    }

    public void Dispose()
    {
        if (isDisposed) return;

        isDisposed = true;

        while (availableInstances.Count > 0)
        {
            T instance = availableInstances.Dequeue();
            if (instance != null)
                UnityEngine.Object.Destroy(instance.gameObject);
        }
    }
}
