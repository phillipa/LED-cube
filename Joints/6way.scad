//All sizes in mm
$fn = 50;
pipe_od = 26.8; // 3/4in pipe
fitting_depth = 25.5; //socket for pipe
foam_thick = 62; //white noodle foam

difference(){
   color("red", 0.3){
      union(){
         cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);

         for(z_rot = [0, 90, 180, 270]){
            rotate([90, 0, z_rot]){
               cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);
            }
         }
         
         rotate([180, 0, 0]){
            cylinder(h = fitting_depth + pipe_od + 5, r=foam_thick/2);
         }
      }
   }

   union(){
      translate([0,0, pipe_od +5]){
         cylinder(h = fitting_depth, r=pipe_od/2);
      }
      cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
      
      for(z_rot = [0, 90, 180, 270]){
         rotate([90, 0, z_rot]){
            translate([0,0, pipe_od +5]){
               cylinder(h = fitting_depth, r=pipe_od/2);
            }
            cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
         }
      }

      rotate([180, 0, 0]){
         translate([0,0, pipe_od +5]){
            cylinder(h = fitting_depth, r=pipe_od/2);
         }
         cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
      }
      
      sphere(r=(foam_thick-8)/2);
   }
}
