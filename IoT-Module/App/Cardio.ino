float getCardio() {
    if (digitalRead(CARDIO_LOMIN_PIN) == 1 || digitalRead(CARDIO_LOPLU_PIN) == 1) {
        return -1;
    } else {
      return (1.0 * analogRead(A0));
    }
}

float getHR() {
    unsigned long currentMillis = millis();
    if (currentMillis - previousMillis >= interval_cardio) {
        previousMillis = currentMillis;
        hitung_HR(analogRead(CARDIO_INPUT_PIN));
    }
    return BPM;
}

void hitung_HR(int data_pulse)
{
    data_now = data_pulse;
    delta_data = data_now - data_old;
    if (delta_data < 0) {
        delta_data = delta_data * -1;
    }
    data_old = data_now;
  
    if (delta_data > 140) { // detek pertama    
        if (flag_detek == false) {      
            flag_detek = true;
            newtime = millis();
        } else {
            beat_time = millis() - newtime;
            if (beat_time < 400) {
                newtime = millis();
  //            beat_time = millis();
            } else {
                HR = 60 / (((float) beat_time) / 1000);
                HR = HR * 0.6 + HR_old * 0.4;
                if (abs(HR - HR_old) > 10) {
                    HR = 60 / (((float) beat_time) / 1000);
                    HR = HR * 0.1 + HR_old * 0.9;
                }
                flag_detek = false;
                HR_old = HR;
            }
        }
    }
  
    // cancel pertitungan
    // 150 BPM = 400 ms dan 40 BPM = 1500 ms
    cek_beat_time = millis() - newtime;
    if (cek_beat_time > 10000) {
        newtime = millis();
        HR = 0;
    }
    BPM = HR;
}
