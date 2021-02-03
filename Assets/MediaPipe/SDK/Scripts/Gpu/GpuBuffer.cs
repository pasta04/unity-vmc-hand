using System;

namespace Mediapipe {
  public class GpuBuffer : MpResourceHandle {
    public GpuBuffer(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) {}

#if UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID
    public GpuBuffer(GlTextureBuffer glTextureBuffer) : base() {
      UnsafeNativeMethods.mp_GpuBuffer__PSgtb(glTextureBuffer.sharedPtr, out var ptr).Assert();
      glTextureBuffer.Dispose(); // respect move semantics
      this.ptr = ptr;
    }
#endif

    protected override void DeleteMpPtr() {
      UnsafeNativeMethods.mp_GpuBuffer__delete(ptr);
    }

#if UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID
    public GlTextureBuffer GetGlTextureBuffer() {
      return new GlTextureBuffer(SafeNativeMethods.mp_GpuBuffer__GetGlTextureBufferSharedPtr(mpPtr), false);
    }
#endif

    public GpuBufferFormat Format() {
      return SafeNativeMethods.mp_GpuBuffer__format(mpPtr);
    }

    public int Width() {
      return SafeNativeMethods.mp_GpuBuffer__width(mpPtr);
    }

    public int Height() {
      return SafeNativeMethods.mp_GpuBuffer__height(mpPtr);
    }
  }
}
