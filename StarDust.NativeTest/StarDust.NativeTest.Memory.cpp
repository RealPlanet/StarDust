#include "pch.h"
#include "CppUnitTest.h"

#include "SDEmitter.h"
#include "TestUtilities.h"
#include "Opcodes.h"
#include "RuntimeInstructionBinder.h"
#include "ExecutionScope.h"
#include "Logger.h"
#include <VirtualMachine.h>
#include <BytecodeParser.h>
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StarDustNativeTest
{
	TEST_CLASS(StarDust_EmitterTest_Memory)
	{
	private:

	public:
		TEST_METHOD(test_stackvalue_string_leak)
		{
			int maxAllocs = 99999;
			std::string a = R"(
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Porta nibh venenatis cras sed felis eget velit. Varius quam quisque id diam vel quam elementum pulvinar etiam. Dictum at tempor commodo ullamcorper a lacus vestibulum. Id diam vel quam elementum. Sed libero enim sed faucibus turpis in. Purus sit amet volutpat consequat mauris. Ante metus dictum at tempor commodo ullamcorper a lacus. Duis at consectetur lorem donec massa sapien faucibus. Duis ut diam quam nulla porttitor. Aliquam sem et tortor consequat id porta. Sed arcu non odio euismod lacinia at quis. Lorem ipsum dolor sit amet consectetur adipiscing. Massa tincidunt dui ut ornare lectus sit amet est placerat. Convallis a cras semper auctor neque vitae tempus quam. Tristique risus nec feugiat in fermentum posuere urna nec. Tellus mauris a diam maecenas sed enim ut sem viverra.

Ut pharetra sit amet aliquam id diam maecenas ultricies. Ornare lectus sit amet est placerat. Mauris vitae ultricies leo integer malesuada nunc vel risus. Ultricies mi quis hendrerit dolor magna eget est lorem. Augue eget arcu dictum varius duis at consectetur lorem donec. Morbi leo urna molestie at elementum eu facilisis. Magna fermentum iaculis eu non diam phasellus vestibulum lorem sed. Amet commodo nulla facilisi nullam. Lorem mollis aliquam ut porttitor leo a diam sollicitudin. Convallis posuere morbi leo urna molestie at elementum eu facilisis. Ullamcorper eget nulla facilisi etiam dignissim diam quis enim lobortis. Sapien pellentesque habitant morbi tristique senectus et. In hendrerit gravida rutrum quisque non tellus. Tellus id interdum velit laoreet id donec ultrices. In nibh mauris cursus mattis molestie.

Lorem mollis aliquam ut porttitor leo a diam. Scelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique. Amet nulla facilisi morbi tempus iaculis urna id volutpat. Sed viverra ipsum nunc aliquet bibendum enim facilisis. Integer malesuada nunc vel risus. Risus nec feugiat in fermentum posuere urna. Elementum curabitur vitae nunc sed velit dignissim sodales ut eu. Egestas pretium aenean pharetra magna. Elementum nibh tellus molestie nunc non blandit massa enim. Id cursus metus aliquam eleifend mi in nulla posuere. Morbi non arcu risus quis varius quam quisque id. Faucibus in ornare quam viverra orci sagittis eu volutpat. Aliquam sem et tortor consequat id porta nibh venenatis. Dolor purus non enim praesent elementum facilisis leo. Facilisi etiam dignissim diam quis enim lobortis scelerisque fermentum dui. Curabitur gravida arcu ac tortor dignissim convallis aenean et. Commodo elit at imperdiet dui accumsan sit amet. Diam sollicitudin tempor id eu.

Malesuada pellentesque elit eget gravida cum. In fermentum posuere urna nec tincidunt praesent. Urna neque viverra justo nec. Sagittis nisl rhoncus mattis rhoncus urna neque viverra justo. Dictum sit amet justo donec enim diam vulputate ut pharetra. Accumsan lacus vel facilisis volutpat est. Tempor nec feugiat nisl pretium fusce. Arcu bibendum at varius vel. Fermentum et sollicitudin ac orci phasellus egestas tellus rutrum tellus. Dictum varius duis at consectetur lorem donec. Fringilla urna porttitor rhoncus dolor purus non enim praesent. Neque convallis a cras semper auctor.

Gravida quis blandit turpis cursus in hac habitasse. Semper auctor neque vitae tempus quam pellentesque nec. Netus et malesuada fames ac turpis egestas maecenas pharetra. At in tellus integer feugiat scelerisque varius morbi enim nunc. In egestas erat imperdiet sed euismod nisi porta lorem mollis. Lacus sed turpis tincidunt id aliquet risus. Mauris commodo quis imperdiet massa tincidunt nunc pulvinar sapien. Rhoncus dolor purus non enim praesent elementum. Sed vulputate odio ut enim. Blandit volutpat maecenas volutpat blandit aliquam etiam erat velit. Arcu felis bibendum ut tristique et egestas. Mollis aliquam ut porttitor leo a diam. Erat velit scelerisque in dictum non. Parturient montes nascetur ridiculus mus. Egestas sed sed risus pretium quam vulputate dignissim. Elementum curabitur vitae nunc sed velit dignissim sodales ut. Erat imperdiet sed euismod nisi porta lorem mollis. Sit amet commodo nulla facilisi nullam.)";

			while (maxAllocs-- > 0)
			{
				sdi::StackValue val(a);
				Assert::AreEqual((int)sdi::StackDataType::String, (int)val.tag);
				Assert::AreEqual(a, val.as_string());
			}
		}
	};
}
