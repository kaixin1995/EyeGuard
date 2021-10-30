# EyeGuard
## 本软件的功能：
> 一.定时关机  
>
> > 本软件允许用户在控制面板中设置一个时间，如果设置的值为负数时则代表不启用自动关机的功能，如果为正数就将会在指定的时、分后进行关机操作，关机前一分就会进行公屏提示。 

> 二.定时休息  
>
> > 用户可以自由设置工作时间以及休息时间，例如您设置工作时间为45分钟，休息时间为3分钟，那么每隔45分钟都将会强制休息3分钟，在即将休息前一分钟就会进行公屏提示。  

> 三.预防痔疮、近视、颈椎病 
>
> > 在到达休息时间内，将会强制对电脑进行锁屏，锁屏状态下，无法进入操作，这个时候您可以起来走动走动，这对预防痔疮、近视以及颈椎病有着非常好的防护作用。

> 四.多种计时模式
>> 本软件拥有多种计时模式，如正常模式、智能计时、游戏模式、加班模式  
> 1.正常模式：正常进行计时  
> 2.智能计时：只有鼠标和键盘进行操作时才会开始进行计时，如果鼠标和键盘超过五分钟没有进行操作，那么就会清零当前工作时间。
> 3.游戏模式:游戏模式和智能计时大部分功能一致，只有部分不同，例如如果检测到当前是全屏状态，那么在即将休息前两秒会自动停止计时，当退出全屏状态下立刻进行休息状态。  
> 4.加班模式:此模式下，强制解锁按钮会一致显示，并且鼠标双击托盘会自动清空当前工作时间。

### 2021/10/30 更新
1.第一版的core版本，因为部分类库的原因，这个版本暂时无法双屏显示锁屏。

 

## 界面
+ 桌面插件  
  ![桌面插件](https://github.com/kaixin1995/EyeGuard/blob/master/Images/%E6%A1%8C%E9%9D%A2%E6%8F%92%E4%BB%B6.png)

+ 提醒页面
  - 点击托盘后工作时间  
    ![点击托盘后工作时间](https://github.com/kaixin1995/EyeGuard/blob/master/Images/%E6%8F%90%E7%A4%BA%E9%9D%A2%E6%9D%BF.png)

  - 休息前提醒页面  
    ![休息前提醒页面](https://github.com/kaixin1995/EyeGuard/blob/master/Images/%E4%BC%91%E6%81%AF%E5%89%8D%E6%8F%90%E9%86%92.png)

+ 设置面板  
  ![设置面板](https://github.com/kaixin1995/EyeGuard/blob/master/Images/%E8%AE%BE%E7%BD%AE%E9%9D%A2%E6%9D%BF.png)

+ 定时关机页面  
  ![定时关机页面](https://github.com/kaixin1995/EyeGuard/blob/master/Images/%E8%87%AA%E5%8A%A8%E5%85%B3%E6%9C%BA.png)


## 本软件使用.net5进行开发，界面使用WPF，界面UI模块使用的是开源UI组件[mahapps](https://mahapps.com/)

