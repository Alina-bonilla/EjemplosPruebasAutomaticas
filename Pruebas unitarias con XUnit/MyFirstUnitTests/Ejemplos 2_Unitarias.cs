using System;
using System.Collections.Generic;
using Xunit;

namespace MyFirstUnitTests
{
    public class Ejemplos_2
    {
        // Arrange (Preparar): Definimos los datos de prueba en una propiedad estática.
        public static IEnumerable<object[]> AdditionData =>
            new List<object[]>
            {
                new object[] { 1, 2, 3 },
                new object[] { -1, -1, -2 },
                new object[] { 0, 0, 0 },
                new object[] { int.MaxValue, 1, int.MinValue } // Esto comprobará la sobrecarga
            };

        [Theory]
        [MemberData(nameof(AdditionData))]
        public void TestAddition(int a, int b, int expected)
        {
            // Arrange (Preparar)
            // En este caso, no hay una configuración compleja adicional ya que los datos se preparan con MemberData.

            // Act (Actuar)
            int result = a + b;

            // Assert (Afirmar)
            Assert.Equal(expected, result);
        }
    }

    public class MemberDataTestExternalMethod
    {
        [Theory]
        [MemberData(
        nameof(ExternalData.GetData),
        10,
        MemberType = typeof(ExternalData))]
        public void Should_be_equal(int value1, int value2, bool shouldBeEqual)
        {
            if (shouldBeEqual)
            {
                Assert.Equal(value1, value2);
            }
            else
            {
                Assert.NotEqual(value1, value2);
            }
        }
        public class ExternalData
        {
            public static TheoryData<int, int, bool> GetData(int start) => new()
             {
             { start, start, true },
             { start, start + 1, false },
             { start + 1, start + 1, true },
             };
                    }
                }
}



