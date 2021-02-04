
// Foam diameter is 62mm
// Other measurements are from one of the 3-way elbows
// or from previous iterations of this design
$fn = 50;
pipe_od = 26.8; // 3/4in pipe
fitting_depth = 25.5; //socket for pipe
foam_thick = 62; //white noodle foam

difference(){
   color("alicewhite", 0.3){
      union(){
         sphere(r=foam_thick/2);

         cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);

         rotate([90, 0, 0]){
            cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);
         }

         rotate([90, 0, 90]){
            cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);
         }
      }
   }

   union(){
      translate([0,0, pipe_od +5]){
         cylinder(h = fitting_depth, r=pipe_od/2);
      }
      cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
      
      rotate([90, 0, 0]){
         translate([0,0, pipe_od +5]){
            cylinder(h = fitting_depth, r=pipe_od/2);
         }
         cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
      }

      rotate([90, 0, 90]){
         translate([0,0, pipe_od +5]){
            cylinder(h = fitting_depth, r=pipe_od/2);
         }
         cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
      }
      
      sphere(r=(foam_thick-8)/2);
   }
}
