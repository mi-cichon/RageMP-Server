 let player = mp.players.local;
 let forklift = null;
 let currentBox = null;
 let attached = false;
 let dropBlip = null;
 let dropMarker = null;
 let dropPosition = null;
 mp.events.addDataHandler("job", (entity, value, oldvalue) => {
     if(entity.type === "player" && entity == player)
     {
        if(oldvalue === "" && value === "forklifts"){

        }
        if(oldvalue === "forklifts" && value === ""){
            if(currentBox != null){
                currentBox.destroy();
            }
            currentBox = null;
            if(dropBlip != null){
                dropBlip.destroy();
            }
            dropBlip = null;
            if(dropMarker != null){
                dropMarker.destroy();
            }
            dropMarker = null;
            forklift = null;
            attached = false;
            dropPosition = null;
        }
    }
 });


 mp.events.add("render", () => {
    if(player.hasVariable("job") && player.getVariable("job") == "forklifts" && forklift != null && mp.vehicles.exists(forklift)){
        if(!attached){
            mp.objects.forEachInStreamRange(box => {
                if(box.hasVariable("boxID") && forklift != null){
                    let pos = forklift.getWorldPositionOfBone(forklift.getBoneIndexByName("forks_attach"));
                    if(getEucDistance(pos, box.position) < 0.5 && pos.z < box.position.z + 0.3){
                        attached = true;
                        let position = box.position;
                        let rotation = box.getRotation(5);
                        let model = box.model;
                        mp.events.callRemote("forkliftBoxPickedUp", box);
                        currentBox = mp.objects.new(model, new mp.Vector3(position.x, position.y, position.z + 2), {
                            alpha: 0
                        });
                        setTimeout(function(){
                            currentBox.setRotation(rotation.x, rotation.y, rotation.z, 5, true);
                        },100);

                        setTimeout(function(){
                            if(forklift != null){
                                currentBox.attachTo(forklift.handle, forklift.getBoneIndexByName("forks_attach"), 0, 0.2, -0.1, 0, 0, 90, true, false, false, false, 0, true);
                                currentBox.setAlpha(255);
                                //currentBox.attachToPhysically(forklift.handle, 0, forklift.getBoneIndexByName("forks_attach"), 0, 1.3, 0.2, 0, 0, 0, 0, 0, 90, 1000000, true, true, false, true, 0);
                                mp.events.callRemote("putItemInHand", "forkliftsBox");
                                dropPosition = dropPositions[getRandomInt(0, dropPositions.length)];
                                dropBlip = mp.blips.new(270, dropPosition, {
                                    scale: 0.7,
                                    color: 15,
                                    name: "Miejsce odstawienia paczki"
                                })
                                dropMarker = mp.markers.new(20, new mp.Vector3(dropPosition.x, dropPosition.y, dropPosition.z + 0.3), 0.7, {
                                    color: [0, 204, 153, 255],
                                    scale: 0.7,
                                    rotation: new mp.Vector3(0, 180, 0)
                                });
                            }
                        }, 1000);
                    }
                }
            });
        }
        else if(dropPosition != null && forklift != null){
            let pos = forklift.getWorldPositionOfBone(forklift.getBoneIndexByName("forks_attach"));
            if(getDistance(dropPosition, pos) < 0.3){
                attached = false;
                dropPosition = null;
                currentBox.destroy();
                currentBox = null;
                dropMarker.destroy();
                dropMarker = null;
                dropBlip.destroy();
                dropBlip = null;
                mp.events.callRemote("putItemInHand", "");
                mp.events.callRemote("forkliftsBoxDropped");
            }
        }
    }
});

mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "forklifts"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                forklift = v;
            }
        }
    }
});

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}


let dropPositions = [
    new mp.Vector3(-525.739, -2401.6865, 12.936881),
    new mp.Vector3(-527.056, -2400.652, 12.939511),
    new mp.Vector3(-535.44147, -2393.5608, 12.939351),
    new mp.Vector3(-536.7528, -2392.4807, 12.937664),
    new mp.Vector3(-538.6617, -2390.9165, 12.93431),
    new mp.Vector3(-540.00793, -2389.8477, 12.93588),
    new mp.Vector3(-555.5193, -2376.6611, 12.936781),
    new mp.Vector3(-556.8705, -2375.6118, 12.93513),
    new mp.Vector3(-558.82886, -2373.9404, 12.935987),
    new mp.Vector3(-563.5873, -2383.412, 12.962486),
    new mp.Vector3(-560.1424, -2372.7832, 12.936795),
    new mp.Vector3(-562.3525, -2384.733, 13.004813),
    new mp.Vector3(-565.24677, -2368.4714, 12.939558),
    new mp.Vector3(-566.5619, -2367.4053, 12.936018),
    new mp.Vector3(-561.8861, -2381.2803, 12.939025),
    new mp.Vector3(-560.4797, -2386.2803, 13.000061),
    new mp.Vector3(-560.35724, -2382.2817, 12.934971),
    new mp.Vector3(-558.5151, -2383.8403, 12.93835),
    new mp.Vector3(-559.217, -2387.4543, 13.01085),
    new mp.Vector3(-557.1708, -2385.0757, 12.933775),
    new mp.Vector3(-541.7486, -2398.246, 12.94534),
    new mp.Vector3(-538.426, -2401.048, 12.946287),
    new mp.Vector3(-537.10614, -2402.0684, 12.940457),
    new mp.Vector3(-535.11053, -2403.6223, 12.933558),
    new mp.Vector3(-575.2045, -2384.0244, 13.010768),
    new mp.Vector3(-533.85767, -2404.8335, 12.94401),
    new mp.Vector3(-573.7807, -2385.1958, 13.010534),
    new mp.Vector3(-533.91315, -2404.801, 14.438265),
    new mp.Vector3(-535.15485, -2403.5667, 14.476442),
    new mp.Vector3(-568.7042, -2389.54, 13.004887),
    new mp.Vector3(-538.39044, -2400.9592, 14.458124),
    new mp.Vector3(-567.30975, -2390.64, 13.010617),
    new mp.Vector3(-565.5477, -2392.2969, 12.966614),
    new mp.Vector3(-537.1065, -2402.1504, 14.4412775),
    new mp.Vector3(-564.1051, -2393.3816, 13.006916),
    new mp.Vector3(-540.30774, -2399.4421, 14.440313),
    new mp.Vector3(-541.65125, -2398.2578, 14.442983),
    new mp.Vector3(-525.5719, -2401.6113, 14.441736),
    new mp.Vector3(-527.0277, -2400.3806, 14.441175),
    new mp.Vector3(-573.74146, -2385.194, 14.470697),
    new mp.Vector3(-535.3714, -2393.505, 14.468738),
    new mp.Vector3(-536.6957, -2392.2576, 14.442882),
    new mp.Vector3(-568.6227, -2389.388, 14.477955),
    new mp.Vector3(-538.5235, -2390.738, 14.443465),
    new mp.Vector3(-539.9172, -2389.61, 14.448438),
    new mp.Vector3(-567.22034, -2390.6208, 14.477008),
    new mp.Vector3(-555.5405, -2376.5835, 14.467788),
    new mp.Vector3(-556.8721, -2375.3433, 14.441056),
    new mp.Vector3(-565.419, -2392.094, 14.477738),
    new mp.Vector3(-558.82446, -2373.9043, 14.474717),
    new mp.Vector3(-560.1213, -2372.7053, 14.460678),
    new mp.Vector3(-563.9461, -2393.3145, 14.477758),
    new mp.Vector3(-565.1753, -2368.3848, 14.440507),
    new mp.Vector3(-566.5834, -2367.2458, 14.449178),
    new mp.Vector3(-559.154, -2387.4993, 14.462997),
    new mp.Vector3(-561.81024, -2381.3035, 14.455107),
    new mp.Vector3(-560.6704, -2386.2876, 14.473741),
    new mp.Vector3(-560.33575, -2382.38, 14.477539),
    new mp.Vector3(-562.41266, -2384.8354, 14.475791),
    new mp.Vector3(-558.61475, -2384.0898, 14.436858),
    new mp.Vector3(-557.18427, -2385.1836, 14.454047),
    new mp.Vector3(-563.77637, -2383.6387, 14.467062),
    new mp.Vector3(-585.1307, -2389.517, 14.47727),
    new mp.Vector3(-583.90625, -2390.6848, 14.460382),
    new mp.Vector3(-543.80066, -2400.5325, 14.477149),
    new mp.Vector3(-581.9963, -2392.271, 14.465433),
    new mp.Vector3(-580.69775, -2393.441, 14.438705),
    new mp.Vector3(-542.1659, -2401.7463, 14.460541),
    new mp.Vector3(-578.8442, -2394.9924, 14.440072),
    new mp.Vector3(-540.46277, -2403.2246, 14.471321),
    new mp.Vector3(-577.4403, -2396.1538, 14.44847),
    new mp.Vector3(-539.02, -2404.365, 14.4547415),
    new mp.Vector3(-577.059, -2386.2961, 14.435129),
    new mp.Vector3(-537.23004, -2405.9248, 14.469351),
    new mp.Vector3(-535.8126, -2407.1802, 14.478089),
    new mp.Vector3(-575.69946, -2387.4927, 14.443751),
    new mp.Vector3(-537.36444, -2415.8186, 14.441346),
    new mp.Vector3(-570.57367, -2391.8384, 14.455813),
    new mp.Vector3(-569.19354, -2392.9, 14.434804),
    new mp.Vector3(-538.8924, -2414.3186, 14.478352),
    new mp.Vector3(-567.3489, -2394.5176, 14.443478),
    new mp.Vector3(-543.8894, -2410.2034, 14.4741125),
    new mp.Vector3(-565.9558, -2395.5388, 14.439855),
    new mp.Vector3(-545.23364, -2408.9873, 14.4774685),
    new mp.Vector3(-552.37335, -2407.1865, 14.480374),
    new mp.Vector3(-553.67395, -2405.989, 14.441391),
    new mp.Vector3(-547.0224, -2407.5906, 14.4715605),
    new mp.Vector3(-550.38617, -2408.7275, 14.438358),
    new mp.Vector3(-549.10834, -2409.8162, 14.440592),
    new mp.Vector3(-548.54834, -2406.239, 14.477937),
    new mp.Vector3(-547.1732, -2411.367, 14.4389),
    new mp.Vector3(-550.2733, -2404.8284, 14.475691),
    new mp.Vector3(-545.8277, -2412.5178, 14.437663),
    new mp.Vector3(-551.7726, -2403.5674, 14.476076),
    new mp.Vector3(-540.6979, -2416.8667, 14.44084),
    new mp.Vector3(-539.3617, -2417.9648, 14.438606),
    new mp.Vector3(-550.8648, -2418.45, 14.441075),
    new mp.Vector3(-552.2276, -2417.3232, 14.44123),
    new mp.Vector3(-543.50543, -2400.5454, 12.974862),
    new mp.Vector3(-554.0926, -2415.7236, 14.443435),
    new mp.Vector3(-542.26074, -2401.6313, 12.980272),
    new mp.Vector3(-555.3898, -2414.639, 14.4456415),
    new mp.Vector3(-540.45197, -2403.2292, 12.980608),
    new mp.Vector3(-538.9875, -2404.411, 12.980507),
    new mp.Vector3(-550.9016, -2418.5151, 12.97689),
    new mp.Vector3(-537.0753, -2405.892, 12.960724),
    new mp.Vector3(-552.2471, -2417.3174, 13.002512),
    new mp.Vector3(-535.7305, -2407.1194, 12.979921),
    new mp.Vector3(-554.11914, -2415.7144, 13.013363),
    new mp.Vector3(-537.4575, -2415.7947, 12.957018),
    new mp.Vector3(-555.4204, -2414.7275, 12.974804),
    new mp.Vector3(-538.8421, -2414.454, 12.980033),
    new mp.Vector3(-545.74414, -2412.4712, 12.965845),
    new mp.Vector3(-547.12885, -2411.368, 12.974575),
    new mp.Vector3(-543.8785, -2410.2488, 12.980247),
    new mp.Vector3(-549.0465, -2409.7786, 12.983595),
    new mp.Vector3(-545.3729, -2408.963, 12.980863),
    new mp.Vector3(-550.4345, -2408.6165, 12.98843),
    new mp.Vector3(-547.146, -2407.5303, 12.980963),
    new mp.Vector3(-552.2482, -2407.0493, 12.963256),
    new mp.Vector3(-548.5702, -2406.3892, 12.977991),
    new mp.Vector3(-553.6634, -2405.946, 12.999266),
    new mp.Vector3(-565.9601, -2395.6418, 13.006957),
    new mp.Vector3(-567.4144, -2394.4524, 13.012131),
    new mp.Vector3(-569.1192, -2392.8093, 12.9661045),
    new mp.Vector3(-570.45966, -2391.7302, 12.966221),
    new mp.Vector3(-550.3888, -2404.806, 12.98059),
    new mp.Vector3(-575.6221, -2387.4316, 12.965341),
    new mp.Vector3(-551.785, -2403.6494, 12.980537),
    new mp.Vector3(-577.0527, -2386.256, 12.986172),
    new mp.Vector3(-583.9976, -2390.729, 12.992058),
    new mp.Vector3(-557.31934, -2416.9646, 12.981021),
    new mp.Vector3(-585.3351, -2389.6404, 12.976329),
    new mp.Vector3(-555.96497, -2418.095, 12.981379),
    new mp.Vector3(-582.0559, -2392.2754, 13.0188265),
    new mp.Vector3(-554.1498, -2419.6975, 12.980929),
    new mp.Vector3(-580.804, -2393.4001, 13.001356),
    new mp.Vector3(-578.83905, -2395.1594, 12.965033),
    new mp.Vector3(-552.7034, -2420.762, 12.979726),
    new mp.Vector3(-577.5597, -2396.1287, 12.992171),
    new mp.Vector3(-557.73083, -2426.6206, 12.979921),
    new mp.Vector3(-593.9213, -2400.005, 12.968737),
    new mp.Vector3(-592.5765, -2401.1248, 12.963481),
    new mp.Vector3(-559.0582, -2425.468, 12.981163),
    new mp.Vector3(-592.6621, -2411.4043, 12.984141),
    new mp.Vector3(-560.93964, -2423.9597, 12.977471),
    new mp.Vector3(-591.2948, -2412.5833, 12.961277),
    new mp.Vector3(-562.23065, -2422.7556, 12.98146),
    new mp.Vector3(-589.33844, -2414.1145, 13.012559),
    new mp.Vector3(-587.98737, -2415.3535, 12.964142),
    new mp.Vector3(-564.129, -2421.2766, 12.978394),
    new mp.Vector3(-565.64667, -2420.0698, 12.962241),
    new mp.Vector3(-586.11975, -2416.8157, 13.008746),
    new mp.Vector3(-557.38464, -2416.9636, 14.476295),
    new mp.Vector3(-584.8437, -2418.0923, 12.965363),
    new mp.Vector3(-555.95874, -2418.1497, 14.476786),
    new mp.Vector3(-581.02527, -2410.8418, 12.978263),
    new mp.Vector3(-579.715, -2412.016, 13.007331),
    new mp.Vector3(-584.32574, -2408.1267, 13.00734),
    new mp.Vector3(-554.2213, -2419.6377, 14.477482),
    new mp.Vector3(-582.93805, -2409.253, 12.980512),
    new mp.Vector3(-552.8124, -2420.799, 14.476179),
    new mp.Vector3(-592.55994, -2401.145, 12.964002),
    new mp.Vector3(-593.98737, -2399.9666, 12.977431),
    new mp.Vector3(-557.62415, -2426.6924, 14.456798),
    new mp.Vector3(-572.43335, -2428.3845, 12.970445),
    new mp.Vector3(-571.14984, -2429.5134, 12.9665),
    new mp.Vector3(-559.0836, -2425.3462, 14.47656),
    new mp.Vector3(-565.966, -2433.7966, 12.988152),
    new mp.Vector3(-560.76953, -2423.7986, 14.47938),
    new mp.Vector3(-564.7409, -2434.7268, 13.000227),
    new mp.Vector3(-562.7804, -2436.4106, 13.002507),
    new mp.Vector3(-562.243, -2422.6074, 14.47837),
    new mp.Vector3(-564.0336, -2421.2056, 14.477267),
    new mp.Vector3(-561.5204, -2437.5198, 12.983467),
    new mp.Vector3(-559.4426, -2428.933, 12.966622),
    new mp.Vector3(-565.55164, -2419.9595, 14.474553),
    new mp.Vector3(-560.80133, -2427.6804, 12.965559),
    new mp.Vector3(-562.81165, -2426.089, 12.971809),
    new mp.Vector3(-577.8471, -2409.8281, 14.437125),
    new mp.Vector3(-564.09985, -2424.9165, 12.964581),
    new mp.Vector3(-579.25714, -2408.4392, 14.477521),
    new mp.Vector3(-565.9795, -2423.408, 12.962491),
    new mp.Vector3(-581.0444, -2407.1387, 14.437703),
    new mp.Vector3(-567.3076, -2422.2803, 12.964638),
    new mp.Vector3(-582.4028, -2405.7495, 14.47981),
    new mp.Vector3(-567.3887, -2422.4124, 14.453475),
    new mp.Vector3(-590.794, -2398.9707, 14.438557),
    new mp.Vector3(-566.0284, -2423.5137, 14.439131),
    new mp.Vector3(-564.1741, -2425.0457, 14.440434),
    new mp.Vector3(-592.14557, -2397.5437, 14.478588),
    new mp.Vector3(-562.842, -2426.2087, 14.450041),
    new mp.Vector3(-587.4039, -2391.8562, 14.478872),
    new mp.Vector3(-560.92395, -2427.9058, 14.4645),
    new mp.Vector3(-559.5303, -2428.8887, 14.443409),
    new mp.Vector3(-561.27576, -2437.4404, 14.483412),
    new mp.Vector3(-562.65533, -2436.5095, 14.44128),
    new mp.Vector3(-564.64215, -2434.8633, 14.443317),
    new mp.Vector3(-566.0479, -2433.7427, 14.447174),
    new mp.Vector3(-571.10004, -2429.3896, 14.445798),
    new mp.Vector3(-572.4188, -2428.3284, 14.440294),
    new mp.Vector3(-584.822, -2417.9807, 14.447076),
    new mp.Vector3(-586.0596, -2416.8708, 14.447509),
    new mp.Vector3(-589.4088, -2414.1118, 14.43857),
    new mp.Vector3(-588.02435, -2415.2307, 14.4399395),
    new mp.Vector3(-592.53705, -2411.4219, 14.442867),
    new mp.Vector3(-591.2216, -2412.5242, 14.438956),
    new mp.Vector3(-592.70636, -2401.0999, 14.437971),
    new mp.Vector3(-593.9268, -2400.0415, 14.441495),
    new mp.Vector3(-585.79987, -2393.1558, 14.4778385),
    new mp.Vector3(-584.2848, -2408.0833, 14.442737),
    new mp.Vector3(-582.919, -2409.2847, 14.441886),
    new mp.Vector3(-583.97736, -2394.6692, 14.477206),
    new mp.Vector3(-579.75555, -2411.9993, 14.4446945),
    new mp.Vector3(-582.5962, -2395.8481, 14.477299),
    new mp.Vector3(-581.0217, -2410.7961, 14.44339),
    new mp.Vector3(-580.699, -2397.4475, 14.479165),
    new mp.Vector3(-579.49725, -2398.5999, 14.478256),
    new mp.Vector3(-579.2995, -2398.4563, 12.971695),
    new mp.Vector3(-580.67847, -2397.3994, 12.973048),
    new mp.Vector3(-582.60944, -2395.778, 12.973405),
    new mp.Vector3(-583.9472, -2394.5557, 12.970884),
    new mp.Vector3(-585.82166, -2393.089, 12.973428),
    new mp.Vector3(-587.2213, -2391.7993, 12.970785),
    new mp.Vector3(-592.0904, -2397.8284, 12.973192),
    new mp.Vector3(-590.68494, -2398.7246, 12.975252),
    new mp.Vector3(-582.47644, -2405.8645, 12.972523),
    new mp.Vector3(-581.0168, -2407.0706, 12.973676),
    new mp.Vector3(-579.2564, -2408.578, 12.973324),
    new mp.Vector3(-612.81946, -2422.372, 14.440724),
    new mp.Vector3(-611.3656, -2423.4783, 14.463788),
    new mp.Vector3(-577.9166, -2409.6055, 12.974343),
    new mp.Vector3(-609.69916, -2425.0044, 14.442136),
    new mp.Vector3(-608.335, -2426.25, 14.446793),
    new mp.Vector3(-606.2914, -2427.8713, 14.441513),
    new mp.Vector3(-604.9683, -2428.93, 14.438463),
    new mp.Vector3(-599.81433, -2433.2134, 14.443993),
    new mp.Vector3(-598.5801, -2434.3784, 14.443033),
    new mp.Vector3(-594.4744, -2413.6277, 12.971848),
    new mp.Vector3(-594.8517, -2427.2302, 14.437074),
    new mp.Vector3(-593.5225, -2428.3694, 14.435874),
    new mp.Vector3(-593.0741, -2414.7708, 12.962393),
    new mp.Vector3(-596.6751, -2425.6833, 14.44115),
    new mp.Vector3(-591.1783, -2416.378, 12.969633),
    new mp.Vector3(-598.11285, -2424.6355, 14.469173),
    new mp.Vector3(-589.8875, -2417.5842, 12.976321),
    new mp.Vector3(-586.14746, -2444.7104, 14.442089),
    new mp.Vector3(-584.8043, -2445.8616, 14.44758),
    new mp.Vector3(-587.9826, -2419.018, 12.957315),
    new mp.Vector3(-579.7471, -2450.1504, 14.443891),
    new mp.Vector3(-586.63855, -2420.2075, 12.970797),
    new mp.Vector3(-578.2376, -2451.2373, 14.467274),
    new mp.Vector3(-573.16705, -2455.4568, 14.474681),
    new mp.Vector3(-574.3762, -2430.5876, 12.973296),
    new mp.Vector3(-571.7433, -2456.6514, 14.464004),
    new mp.Vector3(-572.9461, -2431.7776, 12.974477),
    new mp.Vector3(-574.8599, -2444.1394, 14.471126),
    new mp.Vector3(-567.845, -2436.0159, 12.972762),
    new mp.Vector3(-573.3066, -2445.3096, 14.4440155),
    new mp.Vector3(-571.55566, -2446.7803, 14.439297),
    new mp.Vector3(-566.4297, -2437.0498, 12.9412775),
    new mp.Vector3(-570.1131, -2448.1565, 14.4764805),
    new mp.Vector3(-568.37354, -2449.5962, 14.470563),
    new mp.Vector3(-564.6458, -2438.8276, 12.973947),
    new mp.Vector3(-566.84174, -2450.7522, 14.43862),
    new mp.Vector3(-563.2468, -2439.9312, 12.97421),
    new mp.Vector3(-565.04266, -2448.4158, 12.974301),
    new mp.Vector3(-571.89026, -2456.6812, 12.947331),
    new mp.Vector3(-566.4607, -2447.3623, 12.942152),
    new mp.Vector3(-573.12054, -2455.5776, 12.954693),
    new mp.Vector3(-568.1773, -2445.7375, 12.974865),
    new mp.Vector3(-579.71704, -2450.2185, 12.951553),
    new mp.Vector3(-578.46826, -2451.2644, 12.957851),
    new mp.Vector3(-569.656, -2444.5264, 12.972927),
    new mp.Vector3(-571.5045, -2443.0046, 12.971732),
    new mp.Vector3(-586.2228, -2444.564, 12.962215),
    new mp.Vector3(-584.8049, -2445.7036, 12.963192),
    new mp.Vector3(-572.92957, -2441.9358, 12.942665),
    new mp.Vector3(-574.59784, -2444.1475, 12.946946),
    new mp.Vector3(-573.28564, -2445.2134, 12.9458275),
    new mp.Vector3(-574.47595, -2430.6208, 14.47842),
    new mp.Vector3(-570.081, -2448.0535, 12.952659),
    new mp.Vector3(-572.96796, -2431.8438, 14.477478),
    new mp.Vector3(-571.3174, -2446.9338, 12.951947),
    new mp.Vector3(-567.8471, -2436.1218, 14.477611),
    new mp.Vector3(-568.09515, -2449.6167, 12.951286),
    new mp.Vector3(-566.4705, -2437.404, 14.478647),
    new mp.Vector3(-566.9372, -2450.65, 12.943346),
    new mp.Vector3(-564.8394, -2438.7305, 14.478704),
    new mp.Vector3(-598.4386, -2434.3638, 12.964195),
    new mp.Vector3(-563.2288, -2440.0405, 14.478792),
    new mp.Vector3(-599.9205, -2433.2046, 12.943114),
    new mp.Vector3(-564.92816, -2448.566, 14.440501),
    new mp.Vector3(-604.97876, -2429.0007, 12.944597),
    new mp.Vector3(-566.3744, -2447.1104, 14.478819),
    new mp.Vector3(-606.31885, -2427.808, 12.948801),
    new mp.Vector3(-608.08435, -2426.2563, 12.952343),
    new mp.Vector3(-568.0789, -2445.668, 14.478387),
    new mp.Vector3(-609.54047, -2425.0522, 12.972211),
    new mp.Vector3(-569.6155, -2444.4248, 14.479079),
    new mp.Vector3(-612.8656, -2422.3384, 12.948154),
    new mp.Vector3(-611.60614, -2423.3723, 12.947675),
    new mp.Vector3(-571.42346, -2443.0212, 14.466472),
    new mp.Vector3(-597.9722, -2424.5085, 12.942351),
    new mp.Vector3(-572.8598, -2441.8247, 14.46286),
    new mp.Vector3(-596.6503, -2425.6423, 12.9444275),
    new mp.Vector3(-593.3719, -2428.3618, 12.944339),
    new mp.Vector3(-591.5474, -2426.108, 14.472528),
    new mp.Vector3(-594.6914, -2427.2327, 12.944527),
    new mp.Vector3(-593.0431, -2424.8938, 14.462739),
    new mp.Vector3(-594.7511, -2423.329, 14.480812),
    new mp.Vector3(-618.2578, -2435.4775, 12.940313),
    new mp.Vector3(-596.29895, -2422.2117, 14.447229),
    new mp.Vector3(-616.88446, -2436.4966, 12.951938),
    new mp.Vector3(-610.4837, -2442.0154, 12.95235),
    new mp.Vector3(-594.51733, -2413.823, 14.480333),
    new mp.Vector3(-611.7262, -2440.8203, 12.947579),
    new mp.Vector3(-593.1077, -2414.9675, 14.480395),
    new mp.Vector3(-620.12494, -2444.1218, 12.956098),
    new mp.Vector3(-618.8305, -2445.2332, 12.954044),
    new mp.Vector3(-621.8748, -2442.5928, 12.962344),
    new mp.Vector3(-623.277, -2441.4875, 12.949747),
    new mp.Vector3(-591.24066, -2416.3916, 14.457416),
    new mp.Vector3(-625.2513, -2439.8447, 12.952511),
    new mp.Vector3(-626.5228, -2439.0154, 12.958287),
    new mp.Vector3(-589.89795, -2417.6372, 14.479843),
    new mp.Vector3(-588.1185, -2419.1414, 14.480238),
    new mp.Vector3(-593.4837, -2456.244, 12.945066),
    new mp.Vector3(-586.6023, -2420.4238, 14.480127),
    new mp.Vector3(-594.79645, -2455.046, 12.948476),
    new mp.Vector3(-591.65594, -2457.7803, 12.948076),
    new mp.Vector3(-590.28906, -2458.938, 12.94643),
    new mp.Vector3(-585.63824, -2473.0923, 12.9489355),
    new mp.Vector3(-574.4511, -2430.6663, 14.478384),
    new mp.Vector3(-586.8009, -2471.9927, 12.955216),
    new mp.Vector3(-572.9989, -2431.759, 14.472942),
    new mp.Vector3(-588.99066, -2470.3625, 12.955709),
    new mp.Vector3(-567.90485, -2436.066, 14.476362),
    new mp.Vector3(-590.15814, -2469.2534, 12.95278),
    new mp.Vector3(-566.5598, -2437.3953, 14.478539),
    new mp.Vector3(-592.0655, -2467.5925, 12.959479),
    new mp.Vector3(-593.4175, -2466.6746, 12.960767),
    new mp.Vector3(-564.71405, -2438.7878, 14.478424),
    new mp.Vector3(-563.10364, -2439.8984, 14.437704),
    new mp.Vector3(-587.0344, -2471.9272, 14.441623),
    new mp.Vector3(-585.72577, -2473.102, 14.445833),
    new mp.Vector3(-564.8477, -2448.496, 14.474786),
    new mp.Vector3(-588.7818, -2470.2776, 14.474968),
    new mp.Vector3(-566.2932, -2447.1445, 14.477478),
    new mp.Vector3(-590.2305, -2469.174, 14.44449),
    new mp.Vector3(-592.0687, -2467.5864, 14.455705),
    new mp.Vector3(-568.1502, -2445.6426, 14.4769125),
    new mp.Vector3(-593.48395, -2466.5159, 14.457603),
    new mp.Vector3(-569.642, -2444.502, 14.470753),
    new mp.Vector3(-590.2278, -2459.0493, 14.442364),
    new mp.Vector3(-571.31024, -2442.9446, 14.479358),
    new mp.Vector3(-591.5962, -2457.823, 14.451506),
    new mp.Vector3(-572.75146, -2441.6614, 14.4789915),
    new mp.Vector3(-593.41815, -2456.448, 14.455637),
    new mp.Vector3(-594.6095, -2455.2786, 14.448906),
    new mp.Vector3(-611.6537, -2440.8914, 14.448998),
    new mp.Vector3(-610.5603, -2441.9268, 14.447109),
    new mp.Vector3(-587.83984, -2446.732, 12.993396),
    new mp.Vector3(-618.3214, -2435.2712, 14.446723),
    new mp.Vector3(-586.70496, -2448.1526, 13.0431185),
    new mp.Vector3(-616.96674, -2436.4014, 14.449211),
    new mp.Vector3(-581.5531, -2452.4229, 13.041954),
    new mp.Vector3(-625.15485, -2439.9211, 14.439421),
    new mp.Vector3(-580.237, -2453.5522, 13.042079),
    new mp.Vector3(-626.2917, -2438.845, 14.464812),
    new mp.Vector3(-575.02313, -2457.8176, 13.022654),
    new mp.Vector3(-623.3718, -2441.628, 14.449667),
    new mp.Vector3(-573.6433, -2459.027, 13.036943),
    new mp.Vector3(-622.09375, -2442.6147, 14.448923),
    new mp.Vector3(-620.2836, -2444.1765, 14.446899),
    new mp.Vector3(-588.472, -2456.7043, 13.038215),
    new mp.Vector3(-589.812, -2455.5383, 13.041872),
    new mp.Vector3(-618.84717, -2445.307, 14.442135),
    new mp.Vector3(-591.6023, -2453.9727, 13.042445),
    new mp.Vector3(-593.0295, -2452.7883, 13.042109),
    new mp.Vector3(-608.6443, -2439.9067, 12.994753),
    new mp.Vector3(-609.90967, -2438.656, 13.046482),
    new mp.Vector3(-614.9951, -2434.4128, 13.04442),
    new mp.Vector3(-616.5307, -2433.2283, 13.01988),
    new mp.Vector3(-614.6943, -2424.6067, 13.044142),
    new mp.Vector3(-613.3076, -2425.7874, 13.045849),
    new mp.Vector3(-611.478, -2427.342, 13.046659),
    new mp.Vector3(-610.0658, -2428.423, 13.02953),
    new mp.Vector3(-608.22766, -2430.0833, 13.04739),
    new mp.Vector3(-606.833, -2431.235, 13.044617),
    new mp.Vector3(-601.7155, -2435.5608, 13.046534),
    new mp.Vector3(-600.35913, -2436.7087, 13.047142),
    new mp.Vector3(-608.37354, -2439.6724, 14.482896),
    new mp.Vector3(-609.94147, -2438.4746, 14.482543),
    new mp.Vector3(-615.04926, -2434.4297, 14.440153),
    new mp.Vector3(-616.5156, -2433.0276, 14.479238),
    new mp.Vector3(-614.9011, -2424.5942, 14.481639),
    new mp.Vector3(-613.314, -2425.9656, 14.481758),
    new mp.Vector3(-611.5948, -2427.4905, 14.482283),
    new mp.Vector3(-610.17444, -2428.7375, 14.483554),
    new mp.Vector3(-608.4289, -2430.174, 14.483108),
    new mp.Vector3(-606.87744, -2431.328, 14.48061),
    new mp.Vector3(-601.9232, -2435.5432, 14.483658),
    new mp.Vector3(-600.4682, -2436.7876, 14.48396),
    new mp.Vector3(-588.1834, -2447.1843, 14.477575),
    new mp.Vector3(-586.70233, -2448.3342, 14.478436),
    new mp.Vector3(-581.69977, -2452.4524, 14.480441),
    new mp.Vector3(-580.28625, -2453.6848, 14.479291),
    new mp.Vector3(-575.2989, -2457.8267, 14.478538),
    new mp.Vector3(-573.7532, -2459.1182, 14.478037),
    new mp.Vector3(-588.2723, -2456.6904, 14.478676),
    new mp.Vector3(-589.7258, -2455.4011, 14.478319),
    new mp.Vector3(-591.46063, -2453.9453, 14.478805),
    new mp.Vector3(-593.05347, -2452.6382, 14.478133)
]