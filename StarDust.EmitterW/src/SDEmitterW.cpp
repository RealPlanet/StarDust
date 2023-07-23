#include "SDEmitterW.h"
#include <sstream>

namespace StarDust {
    namespace Emitter {
        void SDEmitterW::Generate(StreamWriter^ outputStream)
        {
            std::stringstream ss;
            theNativeEmitter->generate(&ss);
            String^ output = gcnew String(ss.str().c_str());
            outputStream->Write(output);
        }

        MethodEmitterW^ SDEmitterW::EmitMethod(String^ methodName)
        {
            pin_ptr<const wchar_t> wch = PtrToStringChars(methodName);

            sde::MethodEmitter* ptr = theNativeEmitter->emit_method(wch, methodName->Length);
            return gcnew MethodEmitterW(ptr);
        }
    }
}
