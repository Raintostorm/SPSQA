"""Regression tests for lossless pose extraction."""
import unittest
import numpy as np
from validate_attack_regions import extract


class RegionsTests(unittest.TestCase):
    def setUp(self):
        self.src = np.zeros((30, 60, 4), dtype=np.uint8)
        self.src[4:12, 5:11] = [100, 70, 200, 255]
        self.src[15:18, 23:28] = [100, 180, 200, 255]

    def test_detached_weapon_preserved_in_non_square_frame(self):
        poses = extract(self.src, [dict(rect=[0, 0, 40, 25], anchor=[8, 20])])
        self.assertEqual(poses[0][0][..., 3].sum(), self.src[..., 3].sum())
        self.assertNotEqual(poses[0][0].shape[0], poses[0][0].shape[1])

    def test_missing_part_rejected(self):
        with self.assertRaisesRegex(ValueError, 'Unassigned'):
            extract(self.src, [dict(rect=[0, 0, 20, 25], anchor=[8, 20])])

    def test_cut_edge_rejected(self):
        with self.assertRaisesRegex(ValueError, 'edge'):
            extract(self.src, [dict(rect=[5, 0, 35, 25], anchor=[8, 20])])

    def test_overlap_rejected(self):
        frame = dict(rect=[0, 0, 40, 25], anchor=[8, 20])
        with self.assertRaisesRegex(ValueError, 'shared'):
            extract(self.src, [frame, frame])

    def test_source_boundary_rejected(self):
        with self.assertRaisesRegex(ValueError, 'outside'):
            extract(self.src, [dict(rect=[0, 0, 80, 25], anchor=[8, 20])])

    def test_polygon_cannot_cut_weapon_inside_rectangle(self):
        with self.assertRaisesRegex(ValueError, 'polygon cuts'):
            extract(self.src, [dict(rect=[0, 0, 40, 25], anchor=[8, 20],
                polygon=[[0,0], [25,0], [25,24], [0,24]])])

    def test_seeded_extraction_keeps_detached_part_and_soft_alpha(self):
        self.src[21, 20] = [255, 0, 255, 1]
        p = extract(self.src, [dict(seeds=[[7,7], [24,16]], anchor=[8,20])])
        self.assertEqual(p[0][0][...,3].sum(), self.src[...,3].sum())

    def test_seeded_extraction_rejects_unclaimed_large_part(self):
        self.src[14:24, 22:32] = [100,180,200,255]
        with self.assertRaisesRegex(ValueError, 'Unclaimed'):
            extract(self.src, [dict(seeds=[[7,7]], anchor=[8,20])])


if __name__ == '__main__':
    unittest.main()
