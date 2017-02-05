package im.fhooe.at.golfclubsimulator;
public class Velocity {
    int sampleCounter = 0;
    final int totalSamples = 5;
    long time0, nAccel;
    static int i = 0;
    double aDelT0 = 0, v0 = 0, v = 0;

    final int totalVelocityValues = 1000;
    double[] velocityValues = new double[totalVelocityValues];

    //float []linearAcceleration = new float[3];

    //final int totalAccl = 5;
    double[] accel = new double[totalSamples];

    private double getAvg(double[] a) {
        double total = 0;
        for (int i = 0; i < a.length; i++)
            total = total + a[i];
        return (total / a.length);
    }

    private double getAcceleration(float[] linearAcceleration) {
        return Math.sqrt(Math.pow(linearAcceleration[0], 2) + Math.pow(linearAcceleration[0], 2) + Math.pow(linearAcceleration[0], 2));
    }

    public double getVelocity(float[] linearAcceleration, long time1) {

        //this.linearAcceleration = linearAcceleration;

        try {
            if (sampleCounter < (totalSamples - 1)) {
                if (sampleCounter == 0)
                    time0 = time1;
                accel[sampleCounter] = getAcceleration(linearAcceleration);
                sampleCounter++;
            } else if (sampleCounter == (totalSamples - 1)) {
                accel[sampleCounter] = getAcceleration(linearAcceleration);

                double avgAccel = getAvg(accel);
                long timeDelta = ((time1 - time0) / 1000);
                double aDelT1 = (avgAccel * timeDelta);

                v = calculateVelovity(aDelT1);
                if (i != totalVelocityValues) {
                    velocityValues[i] = v;
                    i++;
                } else {
                    for (int j = 0; j < (totalVelocityValues - 1); j++)
                        velocityValues[j] = velocityValues[j + 1];
                    velocityValues[totalVelocityValues - 1] = v;
                }
                sampleCounter = 0;
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return v;
    }

    private double calculateVelovity(double aDelT1) {
        double v = v0 + (aDelT1 - aDelT0);
        v0 = v;
        aDelT0 = aDelT1;
        return v;
    }


    public double[] getVlArray() {
        return velocityValues;
    }
}